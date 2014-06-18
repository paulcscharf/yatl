using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Linq;

namespace yatl
{
    class BranchingMusicalCompositionParser : Parser
    {
        public BranchingMusicalCompositionParser(StreamReader reader) : base(reader) { }

        /// <summary>
        /// Parse entire stream and return root motif
        /// </summary>
        public Dictionary<string, Motif> ParseFile()
        {
            // Parse motifs into a dictionary
            var motifs = new Dictionary<string, Motif>();
            Motif root = null;
            while (true) {
                this.parseSpace(); // Skip meaningless spaces and linebreaks
                if (EndOfStream)
                    break;
                char c = this.peek();

                switch (c) {
                    case '#':
                        this.parseComment();
                        break;
                    default:
                        Motif motif = this.parseMotif();
                        if (root == null)
                            root = motif;
                        motifs.Add(motif.Name, motif);
                        break;
                }
            }

            // Set successors right for all motifs
            foreach (Motif motif in motifs.Values) {
                motif.Successors = motif.successorNames.Select(key => motifs[key]).ToArray();
            }

            dump(motifs.Values);
            foreach (Motif motif in motifs.Values) {
                Console.WriteLine(motif.ToString());
            }

            return motifs;
        }

        /// <summary>
        /// Read and return exactly one motif
        /// </summary>
        Motif parseMotif()
        {
            string name = this.parseName();
            string[] successorNames = this.parseSuccessorNames();

            // For now we force the toplevel object to be parallel
            Parallel content = (Parallel) this.parseMusicObject();

            return new Motif(name, successorNames, content);
        }

        /// <summary>
        /// Parse and return the motif name
        /// </summary>
        string parseName()
        {
            var name = new StringBuilder();

            while (true) {
                this.parseSpace(); // Skip meaningless spaces and linebreaks
                char c = this.peek();

                switch (c) {
                    case '#':
                        this.parseComment();
                        break;
                    case '>':
                        this.read();
                        return name.ToString();
                    default:
                        this.read();
                        name.Append(c);
                        break;
                }
            }
        }

        /// <summary>
        /// Parse and return the successor names
        /// </summary>
        string[] parseSuccessorNames()
        {
            var name = new StringBuilder();
            var names = new List<string>();

            while (true) {
                this.parseSpace(); // Skip meaningless spaces and linebreaks
                char c = this.peek();

                switch (c) {
                    case '#':
                        this.parseComment();
                        break;
                    case ',':
                        this.read();
                        names.Add(name.ToString());
                        name.Clear();
                        break;
                    case ':':
                        this.read();
                        if (name.Length == 0 && names.Count == 0)
                            throw parseError("Expected successor names");
                        names.Add(name.ToString());
                        return names.ToArray();
                    default:
                        this.read();
                        name.Append(c);
                        break;
                }
            }
        }

        /// <summary>
        /// Parse and throw away comment
        /// </summary>
        void parseComment()
        {
            if (this.read() != '#')
                throw parseError("Expected '#' to parse comment");
            while (!EndOfStream) {
                char c = this.read();
                if (c == '\n') {
                    return;
                }
            }
        }

        /// <summary>
        /// Detect the type of the toplevel MusicObject and parse it
        /// </summary>
        Audible parseMusicObject()
        {
            while (true) {
                // Skip meaningless spaces and linebreaks
                this.parseSpace();
                char c = this.peek();

                switch (c) {
                    case '#':
                        this.parseComment();
                        break;
                    case '{':
                        return this.parseParallel();
                    default:
                        return this.parseSerial();
                }
            }
        }

        /// <summary>
        /// Read and return things
        /// </summary>
        Serial parseSerial()
        {
            var content = new List<Audible>();
            string duration = "";
            string pitchName = "";

            while (true) {
                // Skip spaces and linebreaks
                this.parseSpace();
                if (EndOfStream) {
                    if (content.Count == 0 || duration.Length > 0)
                        throw parseError("Expected music objects");
                    return new Serial(content.ToArray());
                }

                char c = this.peek();

                switch (c) {
                    case '#':
                        this.parseComment();
                        break;
                    case '{':
                        Parallel parallel;
                        if (duration.Length == 0)
                            parallel = this.parseParallel();
                        else {
                            parallel = this.parseParallel(double.Parse(duration));
                            duration = "";
                        }
                        content.Add(parallel);
                        break;
                    case ',':
                        // Return
                        this.read();
                        return new Serial(content.ToArray());
                    case '}':
                        // Return
                        return new Serial(content.ToArray());
                    default:
                        if (char.IsDigit(c) || c == '.') {
                            // Must be part of duration,
                            // because notes don't start with a digit
                            duration = this.parseDouble();
                        }
                        else {
                            pitchName = this.parseWord();

                            // We're done parsing a note, so let's construct it
                            try {
                                Pitch pitch = Pitch.FromString(pitchName);
                                Note note;
                                if (duration.Length == 0)
                                    note = new Note(1, pitch);
                                else {
                                    note = new Note(double.Parse(duration), pitch);
                                    duration = "";
                                }
                                content.Add(note);
                            }
                            catch (KeyNotFoundException) {
                                throw parseError("Don't know pitch '" + pitchName + "'");
                            }
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// Read and return things
        /// </summary>
        Parallel parseParallel(double durationMultiplier = 1)
        {
            var content = new List<Audible>();

            if (this.read() != '{')
                throw parseError("Expected '{' for parsing parallel");
            content.Add(this.parseSerial());


            while (true) {
                // Skip spaces and linebreaks
                this.parseSpace();
                if (EndOfStream) {
                    if (content.Count == 0)
                        throw parseError("Expected music objects");
                    return new Parallel(content.ToArray(), durationMultiplier);
                }

                char c = this.peek();

                switch (c) {
                    case '#':
                        this.parseComment();
                        break;
                    case '}':
                        // Return
                        this.read();
                        return new Parallel(content.ToArray(), durationMultiplier);
                    default:
                        content.Add(this.parseSerial());
                        break;
                }
            }
        }
    }
}