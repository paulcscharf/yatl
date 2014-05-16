using System;
using System.Collections.Generic;
using System.IO;

namespace yatl
{
    class MusicalCompositionGraph
    {
        Motif root;

        public MusicalCompositionGraph FromFile(string filename)
        {
            int counter = 0;
            string line;

            // Read the file and display it line by line.
            StreamReader file = new StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                Motif motif = Motif.FromString(line);
                counter++;
            }

            file.Close();
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
