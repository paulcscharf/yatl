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
        public ParseError(string message, int line, int column)
        : base(string.Format("At {0},{1}: {2}", line, column, message))
        {
        }
    }

    abstract class Parser
    {
        int line = 0;
        int column = 0;
        StreamReader reader;

        public bool EndOfStream
        {
            get
            {
                return this.reader.EndOfStream;
            }
        }

        public Parser(StreamReader reader)
        {
            this.reader = reader;
        }


        protected ParseError parseError(string message)
        {
            return new ParseError(message, this.line, this.column);
        }

        protected char peek()
        {
            int c = this.reader.Peek();
            if (c == -1)
                throw this.parseError("Unexpected end of file.");
            return (char) c;
        }

        protected char read()
        {
            this.column++;
            int c = this.reader.Read();
            if (c == -1)
                throw this.parseError("Unexpected end of file.");
            return (char) c;
        }

        /// <summary>
        /// Parse sequence of alpha numberic characters
        /// </summary>
        protected string parseWord()
        {
            StringBuilder word = new StringBuilder();
            while (!EndOfStream)
            {
                char c = (char) this.peek();
                if (!char.IsLetterOrDigit(c))
                {
                    if (word.Length == 0)
                        throw parseError("Expected alphanumerical characters");
                    return word.ToString();
                }
                this.read();
                word.Append(c);
            }

            if (word.Length == 0)
                throw parseError("Unexpected EOF");
            return word.ToString();
        }

        /// <summary>
        /// Parse sequence of alpha numberic characters
        /// </summary>
        protected string parseInteger()
        {
            StringBuilder word = new StringBuilder();
            while (!EndOfStream)
            {
                char c = (char) this.peek();
                if (!char.IsDigit(c))
                {
                    if (word.Length == 0)
                        throw parseError("Expected numerical characters");
                    return word.ToString();
                }
                this.read();
                word.Append(c);
            }

            if (word.Length == 0)
                throw parseError("Unexpected EOF");
            return word.ToString();
        }

        /// <summary>
        /// Parse spaces and newlines, then return void
        /// </summary>
        protected void parseSpace()
        {
            while (!this.EndOfStream)
            {
                switch ((char) this.peek())
                {
                case ' ':
                    this.read();
                    break;
                case '\n':
                    this.read();
                    break;
                default:
                    return;
                }
            }
        }

        /// <summary>
        /// Print given object
        /// If instance of IEnumerable, do it recursively
        /// </summary>
        public static void dump(object o)
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
            catch (Exception)
            {
                Console.Write(o.ToString());
            }
            Console.WriteLine();
        }
    }
}
