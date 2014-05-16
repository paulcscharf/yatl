using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace yatl
{
    class BranchingMusicalComposition
    {
        Motif root;

        public BranchingMusicalComposition FromFile(string filename)
        {
            // Parse file and return a BranchingMusicalComposition
            string line;

            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string sMotif = BranchingMusicalCompositionParser.ReadMotif(reader);
                    Motif motif = Motif.FromString(sMotif);
                }
            }

            throw new NotImplementedException();
        }
    }

    class Motif
    {
        public IEnumerable<Motif> Successors
        {
            get;
            private set;
        }

        public static Motif FromString(string s)
        {
            throw new NotImplementedException();
        }
    }
}
