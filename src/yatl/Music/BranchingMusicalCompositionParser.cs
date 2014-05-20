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

        public BranchingMusicalCompositionParser(StreamReader reader): base(reader)
        {
        }

        /// <summary>
        /// Parse entire stream and return root motif
        /// </summary>
        public Motif ParseFile()
        {
            // Parse motifs into a dictionary
            var motifs = new Dictionary<string, Motif>();
            Motif root = null;
            while (true)
            {
                Motif motif = this.parseMotif();
                if (motif == null)
                    break;
                if (root == null)
                    root = motif;
                motifs.Add(motif.Name, motif);
            }

            // Set successors right for all motifs
            foreach (Motif motif in motifs.Values)
            {
                motif.Successors = motif.successorNames.Select(key => motifs[key]).ToList();
            }

            dump(motifs.Values);
            foreach (Motif motif in motifs.Values)
            {
                Console.WriteLine(motif.ToString());
            }

            return root;
        }

        /// <summary>
        /// Read and return exactly one motif
        /// </summary>
        Motif parseMotif()
        {
            string name = this.parseName();
            if (name == null)
                return null;
            string[] successorNames = this.parseSuccessorNames();
            MusicObject content = this.parseMusicObject();

            return new Motif(name, successorNames, content);
        }

        /// <summary>
        /// Parse and return the motif name
        /// </summary>
        string parseName()
        {
            var name = new StringBuilder();

            while (!this.EndOfStream)
            {
                this.parseSpace(); // Skip meaningless spaces and linebreaks
                char c = this.peek();

                switch (c)
                {
                case '#':
                    this.parseComment();
                    break;
                case '>':
                    this.read();
                    return name.ToString();
                    break;
                default:
                    this.read();
                    name.Append(c);
                    break;
                }
            }
            if (name.Length > 0)
                throw parseError("Expected '->'");
            return null;
        }

        /// <summary>
        /// Parse and return the successor names
        /// </summary>
        string[] parseSuccessorNames()
        {
            var name = new StringBuilder();
            var names = new List<string>();

            while (!this.EndOfStream)
            {
                this.parseSpace(); // Skip meaningless spaces and linebreaks
                char c = this.peek();

                switch (c)
                {
                case '#':
                    this.parseComment();
                    break;
                case ',':
                    names.Add(name.ToString());
                    name.Clear();
                    this.read();
                    break;
                case ':':
                    this.read();
                    if (name.Length == 0 && names.Count == 0)
                        throw parseError("Expected successor names");
                    names.Add(name.ToString());
                    return names.ToArray();
                default:
                    name.Append(c);
                    break;
                }
            }

            throw parseError("Expected ':'");
        }

        /// <summary>
        /// Parse and throw away comment
        /// </summary>
        void parseComment()
        {
            if (this.read() != '#')
                throw parseError("Expected '#' to parse comment");
            while (!this.EndOfStream)
            {
                char c = this.read();
                if (c == '\n')
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Detect the type of the toplevel MusicObject and parse it
        /// </summary>
        MusicObject parseMusicObject()
        {
            while (!this.EndOfStream)
            {
                // Skip meaningless spaces and linebreaks
                this.parseSpace();
                char c = this.peek();

                switch (c)
                {
                case '#':
                    this.parseComment();
                    break;
                case '{':
                    return this.parseParallel();
                    break;
                default:
                    return this.parseSerial();
                }
            }
            throw parseError("Unexpected EOF");
        }

        /// <summary>
        /// Read and return things
        /// </summary>
        Serial parseSerial()
        {
            var content = new List<MusicObject>();
            string duration = "";
            string pitchName = "";
            char c;

            while (!this.EndOfStream)
            {
                // Skip meaningless spaces and linebreaks
                this.parseSpace();
                c = this.peek();

                switch (c)
                {
                case '#':
                    this.parseComment();
                    break;
                case '{':
                    Parallel parallel = this.parseParallel();
                    content.Add(parallel);
                    break;
                case ',':
                    // Return
                    return new Serial(content.ToArray());
                    break;
                default:
                    if (char.IsDigit(c))
                    {
                        if (pitchName.Length == 0)
                        {
                            // Must be part of duration,
                            // because notes don't start with a digit
                            duration = this.parseWord();
                        }
                    }
                    else
                    {
                        pitchName = this.parseWord();

                        // We're done parsing a note, so let's construct it
                        try
                        {
                            Pitch pitch = Pitch.FromString(pitchName);
                            Note note;
                            if (duration.Length == 0)
                                note = new Note(1, pitch);
                            else
                                note = new Note(int.Parse(duration), pitch);
                            content.Add(note);
                        }
                        catch (KeyNotFoundException e)
                        {
                            throw parseError("Don't know pitch " + pitchName);
                        }
                    }
                    break;
                }
            }
            throw parseError("Unexpected EOF");
        }
        /// <summary>
        /// Read and return things
        /// </summary>
        Parallel parseParallel()
        {
            throw new NotImplementedException();
        }
    }
}
