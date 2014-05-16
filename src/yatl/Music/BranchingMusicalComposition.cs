using System;
using System.Collections.Generic;
using System.IO;

namespace yatl
{
    class BranchingMusicalComposition
    {
        Motif root;

        public BranchingMusicalComposition FromFile(string filename)
        {
            // Parses file and returns a BranchingMusicalComposition
            int counter = 0;
            string line;

            using (StreamReader file = new StreamReader(filename))
            {
                while ((line = file.ReadLine()) != null)
                {
                    Motif motif = Motif.FromString(line);
                    counter++;
                }
            }

            return null;
        }
    }

    class Motif
    {
        public IEnumerable<Motif> Successors { get; private set; }

        public static Motif FromString(string s){
            throw new NotImplementedException();
        }
    }
}
