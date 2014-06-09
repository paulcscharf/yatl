using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yatl
{
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

        public override IEnumerable<SoundEvent> Render(RenderParameters parameters)
        {
            return musicContent.Render(parameters);
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
