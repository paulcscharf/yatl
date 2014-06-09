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
        public readonly Motif Root;

        public BranchingMusicalComposition(string filename)
        {
            // Parse file and return a composition
            using (var reader = new StreamReader(filename)) {
                var parser = new BranchingMusicalCompositionParser(reader);
                this.Root = parser.ParseFile();
            }
            Console.WriteLine("Succesfully parsed " + filename);
        }
    }
}