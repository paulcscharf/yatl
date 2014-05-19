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
            string content = this.parseMusicObject().ToString();

            return new Motif(name, successorNames, content);
        }

        /// <summary>
        /// Parse and return the motif name
        /// </summary>
        string parseName()
        {
            var name = new StringBuilder();
            char c;
            bool dash = false;

            while (!this.EndOfStream)
            {
                c = this.read();

                switch (c)
                {
                case '#':
                    this.parseComment();
                    break;
                case ' ':
                    break;
                case '\n':
                    break;
                case '-':
                    dash = true;
                    break;
                case '>':
                    if (dash)
                        return name.ToString();
                    else
                        parseError("Unexpected '>'");
                    break;
                default:
                    name.Append(c);
                    break;
                }
            }
            if (name.Length > 0)
                parseError("Expected '->'");
            return null;
        }

        /// <summary>
        /// Parse and return the successor names
        /// </summary>
        string[] parseSuccessorNames()
        {
            var name = new StringBuilder();
            var names = new List<string>();
            char c;

            while (!this.EndOfStream)
            {
                c = this.read();
                switch (c)
                {
                case '#':
                    this.parseComment();
                    break;
                case ' ':
                    break;
                case '\n':
                    break;
                case ',':
                    names.Add(name.ToString());
                    name.Clear();
                    break;
                case ':':
                    if (name.Length == 0 && names.Count == 0)
                        parseError("Expected successor names");
                    names.Add(name.ToString());
                    return names.ToArray();
                default:
                    name.Append(c);
                    break;
                }
            }
            // How to fix this??????/
            parseError("Expected ':'");
            return null;
        }

        /// <summary>
        /// Parse and throw away comment
        /// </summary>
        void parseComment()
        {
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
        /// Read and return motif content
        /// </summary>
        MusicObject parseMusicObject()
        {
            string duration;
            int level = 0;
            char c;

            while (!this.EndOfStream)
            {
                c = this.peek();

                if (c == ',' && level == 0)
                    throw new NotImplementedException("");

                switch (c)
                {
                case '#':
                    this.parseComment();
                    break;
                case '{':
                    level++;
                    break;
                case '}':
                    if (level <= 0)
                        parseError("Unexpected '}'");
                    else
                        level--;
                    break;
                default:
                    if (true) //if alphabet char
                    {
                        string pitchName = this.parseWord();
                        try
                        {
                            Pitch pitch = Pitch.FromString(pitchName);
                        }
                        catch (KeyNotFoundException e)
                        {
                            parseError("Don't know pitch " + pitchName);
                        }
                    }
                    break;
                }
            }

            if (level != 0)
                parseError("Expected '}'");
            return null;
        }
    }
}
