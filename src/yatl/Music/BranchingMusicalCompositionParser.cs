using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

        void debug(object o)
        {
            Console.WriteLine(o.ToString());
        }

        public Motif ParseMotif(StreamReader reader)
        {
            // Read and return string containing exactly one motif

            string name = this.ParseName(reader);
            if (name == null)
                return null;
            string[] successorNames = this.ParseSuccessorNames(reader);
            string content = this.ParseContent(reader);

            return new Motif(name, successorNames, content);

        }

        public string ParseName(StreamReader reader)
        {
            // Parse and return the motif name
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

        public string[] ParseSuccessorNames(StreamReader reader)
        {
            // Parse and return the successor names
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
                    names.Add(name.ToString());
                    return names.ToArray();
                default:
                    name.Append(c);
                    break;
                }
            }
            throw new ParseError("Expected ':'", this.currentLine);
        }

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

        public string ParseContent(StreamReader reader)
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

        /*
        public Motif ParseMotifOld(string sMotif)
        {
            string[] splitMotif = sMotif.Split(new string[] {":"}, StringSplitOptions.None);
            if (splitMotif.Length > 2)
                throw new ParseError("Too many ':'", this.currentLine);
            if (splitMotif.Length < 2)
                throw new ParseError("Expected ':'", this.currentLine);

            string label = splitMotif[0];
            string content = splitMotif[1];

            string[] splitLabel = label.Split(new string[] {"->"}, StringSplitOptions.None);
            if (splitLabel.Length > 2)
                throw new ParseError("Too many '->'", this.currentLine);
            if (splitLabel.Length < 2)
                throw new ParseError("Expected '->'", this.currentLine);

            string name = splitLabel[0].Trim();
            string[] successorNames = splitLabel[1].Trim().Split(',');

            return new Motif(name, successorNames, content);
        }
        */
    }
}
