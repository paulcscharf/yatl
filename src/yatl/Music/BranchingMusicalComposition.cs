using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Cireon.Audio;
using yatl.Environment;

namespace yatl
{
    class BranchingMusicalComposition
    {
        public readonly Dictionary<string, Motif> Motifs = new Dictionary<string, Motif>();

        public BranchingMusicalComposition(string filename)
        {
            // Parse file and return a composition
            using (var reader = new StreamReader(filename)) {
                var parser = new BranchingMusicalCompositionParser(reader);
                this.Motifs = parser.ParseFile();
            }
            //Console.WriteLine("Succesfully parsed " + filename);
        }
    }
}
