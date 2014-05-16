using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace yatl
{
    class BranchingMusicalComposition
    {
        Motif root;

        public static BranchingMusicalComposition FromFile(string filename)
        {
            // Parse file and return a BranchingMusicalComposition
            var parser = new BranchingMusicalCompositionParser();
            var motifs = new List<Motif>();

            using (var reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string sMotif = parser.ReadMotif(reader);
                    Motif motif = parser.ParseMotif(sMotif);
                    motifs.Add(motif);
                }
            }

            Console.WriteLine("Succesfully parsed " + filename);
            foreach (var motif in motifs)
            {
                Console.WriteLine(motif.ToString());
            }

            return null;
        }
    }

    class Motif
    {
        public IEnumerable<Motif> Successors
        {
            get;
            private set;
        }
        public string Name
        {
            get;
            private set;
        }
        string[] successorNames;
        string musicContent;

        public Motif(string name, string[] successorNames, string musicContent)
        {
            this.Name = name;
            this.successorNames = successorNames;
            this.musicContent = musicContent;
        }

        public string ToString()
        {
            return this.Name + " -> " + string.Join(", ", this.successorNames)
                + ": " + this.musicContent;
        }
    }
}
