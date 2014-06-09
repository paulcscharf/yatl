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
        Parallel voices;

        public Motif(string name, string[] successorNames, Parallel voices)
        {
            this.Name = name;
            this.successorNames = successorNames;
            this.voices = voices;
        }

        public override IEnumerable<SoundEvent> Render(RenderParameters parameters)
        {
            return voices.Render(parameters);
        }

        public override double Duration { get { return this.voices.Duration; } }

        public override string ToString()
        {
            var successorNames = this.Successors.Select(motif => motif.Name);
            return this.Name + " -> " + string.Join(",", successorNames)
                   + ": " + this.voices.ToString();
        }
    }
}
