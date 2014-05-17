using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Linq;

namespace yatl
{
    class ParseError : Exception
    {
        public ParseError(string message, int lineNumber)
        : base("At line " + lineNumber.ToString() + ": " + message)
        {
        }
    }

    class BranchingMusicalCompositionParser
    {
        int currentLine = 0;

        /// <summary>
        /// Print given object
        /// If instance of IEnumerable, do it recursively
        /// </summary>
        static void dump(object o)
        {
            try
            {
                var list = (IEnumerable)o;
                Console.Write("{");
                foreach (var item in list)
                {
                    dump(item);
                    Console.Write(", ");
                }
                Console.Write("}");
            }
            catch (Exception e)
            {
                Console.Write(o.ToString());
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Parse file and return root motif
        /// </summary>
        public Motif ParseFile(string filename)
        {
            // Parse motifs into a dictionary
            var motifs = new Dictionary<string, Motif>();
            Motif root = null;
            using (var reader = new StreamReader(filename))
            {
                while (true)
                {
                    Motif motif = this.ParseMotif(reader);
                    if (motif == null)
                        break;
                    if (root == null)
                        root = motif;
                    motifs.Add(motif.Name, motif);
                }
            }

            // Set successors right for all motifs
            foreach (Motif motif in motifs.Values)
            {
                motif.Successors = motif.successorNames.Select(key => motifs[key]).ToList();
            }

            Console.WriteLine("Succesfully parsed " + filename);
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
        public Motif ParseMotif(StreamReader reader)
        {
            string name = this.ParseName(reader);
            if (name == null)
                return null;
            string[] successorNames = this.ParseSuccessorNames(reader);
            string content = this.ParseMusic(reader);

            return new Motif(name, successorNames, content);
        }

        /// <summary>
        /// Parse and return the motif name
        /// </summary>
        public string ParseName(StreamReader reader)
        {
            var name = new StringBuilder();
            char c;
            bool dash = false;

            while (!reader.EndOfStream)
            {
                c = (char) reader.Read();
                switch (c)
                {
                case '#':
                    this.ParseComment(reader);
                    break;
                case ' ':
                    break;
                case '\n':
                    this.currentLine++;
                    break;
                case '-':
                    dash = true;
                    break;
                case '>':
                    if (dash)
                        return name.ToString();
                    else
                        throw new ParseError("Unexpected '>'", this.currentLine);
                    break;
                default:
                    name.Append(c);
                    break;
                }
            }
            if (name.Length > 0)
                throw new ParseError("Expected '->'", this.currentLine);
            return null;
        }

        /// <summary>
        /// Parse and return the successor names
        /// </summary>
        public string[] ParseSuccessorNames(StreamReader reader)
        {
            var name = new StringBuilder();
            var names = new List<string>();
            char c;

            while (!reader.EndOfStream)
            {
                c = (char) reader.Read();
                switch (c)
                {
                case '#':
                    this.ParseComment(reader);
                    break;
                case ' ':
                    break;
                case '\n':
                    this.currentLine++;
                    break;
                case ',':
                    names.Add(name.ToString());
                    name.Clear();
                    break;
                case ':':
                    if (name.Length == 0 && names.Count == 0)
                        throw new ParseError("Expected successor names", this.currentLine);
                    names.Add(name.ToString());
                    return names.ToArray();
                default:
                    name.Append(c);
                    break;
                }
            }
            throw new ParseError("Expected ':'", this.currentLine);
        }

        /// <summary>
        /// Parse and throw away comment
        /// </summary>
        public void ParseComment(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                char c = (char) reader.Read();
                if (c == '\n')
                {
                    this.currentLine++;
                    return;
                }
            }
        }

        /// <summary>
        /// Read and return motif content
        /// </summary>
        public string ParseMusic(StreamReader reader)
        {
            StringBuilder sb = new StringBuilder();
            int level = 0;
            char c;

            while (!reader.EndOfStream)
            {
                c = (char) reader.Read();

                if (c == ',' && level == 0)
                    return sb.ToString();

                switch (c)
                {
                case '#':
                    this.ParseComment(reader);
                    break;
                case '\n':
                    this.currentLine++;
                    break;
                case '{':
                    level++;
                    break;
                case '}':
                    if (level <= 0)
                        throw new ParseError("Unexpected '}'", this.currentLine);
                    else
                        level--;
                    break;
                default:
                    sb.Append(c);
                    break;
                }
            }

            if (level != 0)
                throw new ParseError("Expected '}'", this.currentLine);
            return sb.ToString();
        }
    }
}
