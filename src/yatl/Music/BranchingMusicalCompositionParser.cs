using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace yatl
{
    class BranchingMusicalCompositionParser
    {
        public static string ReadMotif(StreamReader reader)
        {
            // Read and return string containing exactly one motif
            StringBuilder sb = new StringBuilder();
            int level = 0;

            while (!reader.EndOfStream)
            {
                char c = (char) reader.Read();
                if (c == ',' && level == 0)
                    break;
                else if (c == '{')
                    level++;
                else if (c == '}')
                {
                    if (level == 0)
                        throw new Exception("Unexpected '}'");
                    else
                        level--;
                }
                sb.Append(c);
            }

            return sb.ToString();
        }

        public static void ParseLabel(string sMotif)
        {

        }
    }
}
