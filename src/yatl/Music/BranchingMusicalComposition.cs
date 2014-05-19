using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace yatl
{
    class BranchingMusicalComposition
    {
        public Motif root;

        public static BranchingMusicalComposition FromFile(string filename)
        {
            // Parse file and return a BranchingMusicalComposition
            var self = new BranchingMusicalComposition();
            using (var reader = new StreamReader(filename))
            {
                var parser = new BranchingMusicalCompositionParser(reader);
                self.root = parser.ParseFile();
            }
            Console.WriteLine("Succesfully parsed " + filename);
            return self;
        }
    }

    class Motif
    {
        public IEnumerable<Motif> Successors
        {
            get;
            set;
        }
        public string Name
        {
            get;
            private set;
        }
        public string[] successorNames;
        string musicContent;

        public Motif(string name, string[] successorNames, string musicContent)
        {
            this.Name = name;
            this.successorNames = successorNames;
            this.musicContent = musicContent;
        }

        public string ToString()
        {
            var successorNames = this.Successors.Select(motif => motif.Name);
            return this.Name + " -> " + string.Join(",", successorNames)
                   + ": " + this.musicContent;
        }
    }
}
