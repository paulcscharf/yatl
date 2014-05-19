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

        public Parser(StreamReader reader)
        {
            this.reader = reader;
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
            catch (Exception e)
            {
                Console.Write(o.ToString());
            }
            Console.WriteLine();
        }

        protected void parseError(string message)
        {
            throw new ParseError(message, this.line, this.column);
        }

        void parseNewlines()
        {
            while (true)
            {
                char c = (char) this.reader.Peek();
                if (c == '\n')
                {
                    this.column = 0;
                    this.line++;
                    this.reader.Read();
                    c = (char) this.reader.Peek();
                }
                else
                    return;
            }
        }

        protected char peek()
        {
            return (char) this.reader.Peek();
        }

        protected char read()
        {
            this.parseNewlines();
            this.column++;
            return (char) this.reader.Read();
        }

        public bool EndOfStream
        {
            get
            {
                return this.reader.EndOfStream;
            }
        }

        /// <summary>
        /// Parse characters until space or newline, then return string
        /// </summary>
        protected string parseWord()
        {
            StringBuilder word = new StringBuilder();
            while (!reader.EndOfStream)
            {
                switch ((char) reader.Peek())
                {
                case ' ':
                    return word.ToString();
                    break;
                case '\n':
                    return word.ToString();
                    break;
                default:
                    word.Append((char)reader.Read());
                    break;
                }
            }
            if (word.Length == 0)
                this.parseError("Unexpected EOF");
            return word.ToString();
        }

        /// <summary>
        /// Parse consume spaces and newlines, then return
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
    }
}
