using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace yatl
{
    class BranchingMusicalComposition
    {
        public readonly Motif Root;

        public BranchingMusicalComposition(string filename)
        {
            // Parse file and return a composition
            using (var reader = new StreamReader(filename))
            {
                var parser = new BranchingMusicalCompositionParser(reader);
                this.Root = parser.ParseFile();
            }
            Console.WriteLine("Succesfully parsed " + filename);
        }
    }

    class Motif : Audible
    {
        public Motif[] Successors { get; set; }
        public readonly string Name;
        public readonly string[] successorNames;
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
