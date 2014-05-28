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

    class Motif : Audible
    {
        public IEnumerable<Motif> Successors { get; set; }
        public string Name { get; private set; }
        public string[] successorNames;
        Audible musicContent;

        public Motif(string name, string[] successorNames, Audible musicContent)
        {
            this.Name = name;
            this.successorNames = successorNames;
            this.musicContent = musicContent;
        }

        public override IEnumerable<SoundEvent> Render()
        {
            return musicContent.Render();
        }

        public override double Duration { get { return this.musicContent.Duration; } }

        public override string ToString()
        {
            var successorNames = this.Successors.Select(motif => motif.Name);
            return this.Name + " -> " + string.Join(",", successorNames)
                   + ": " + this.musicContent.ToString();
        }
    }
}
