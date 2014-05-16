using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace yatl
{
    class ParseException : Exception
    {
        public ParseException(string message, int lineNumber)
        : base("At line " + lineNumber.ToString() + ": " + message)
        {
        }
    }

    class BranchingMusicalCompositionParser
    {
        int currentLine = 0;

        public string ReadMotif(StreamReader reader)
        {
            // Read and return string containing exactly one motif
            StringBuilder sb = new StringBuilder();
            int level = 0;
            bool comment = false;

            while (!reader.EndOfStream)
            {
                char c = (char) reader.Read();
                if (c == ',' && level == 0)
                    break;
                if (c == '#')
                    comment = true;
                if (c == '\n')
                {
                    this.currentLine++;
                    comment = false;
                    continue;
                }
                else if (c == '{')
                    level++;
                else if (c == '}')
                {
                    if (level == 0)
                        throw new ParseException("Unexpected '}'", this.currentLine);
                    else
                        level--;
                }
                sb.Append(c);
            }

            if (level != 0)
                throw new ParseException("Expected '}'", this.currentLine);
            return sb.ToString();
        }

        public Motif ParseMotif(string sMotif)
        {
            string[] splitMotif = sMotif.Split(new string[] {":"}, StringSplitOptions.None);
            if (splitMotif.Length > 2)
                throw new ParseException("Too many ':'", this.currentLine);
            if (splitMotif.Length < 2)
                throw new ParseException("Expected ':'", this.currentLine);

            string label = splitMotif[0];
            string content = splitMotif[1];

            string[] splitLabel = label.Split(new string[] {"->"}, StringSplitOptions.None);
            if (splitLabel.Length > 2)
                throw new ParseException("Too many '->'", this.currentLine);
            if (splitLabel.Length < 2)
                throw new ParseException("Expected '->'", this.currentLine);

            string name = splitLabel[0].Trim();
            string[] successorNames = splitLabel[1].Trim().Split(',');

            return new Motif(name, successorNames, content);
        }
    }
}
