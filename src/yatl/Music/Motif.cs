using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using yatl.Utilities;

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
            // First generate arpeggios
            var arpeggios = new List<Note>();
            double start = 0;
            foreach (var basenote in this.voices.Content[1].Content) {
                double end = start + basenote.Duration;

                // Gather arpeggio set
                var arpeggioSpace = new List<Pitch>();
                foreach (var voice in this.voices.Content) {
                    foreach (var pitch in voice.GetRange(start, end).Select(note => note.Pitch)) {
                        arpeggioSpace.Add(pitch);
                        arpeggioSpace.Add(pitch.NextOctave());
                        arpeggioSpace.Add(pitch.PreviousOctave());
                    }
                }

                // Select tones
                var chord = arpeggioSpace.SelectRandom(6).ToList();
                chord.Sort();
                double duration = basenote.Duration / (double) chord.Count;
                arpeggios.AddRange(chord.Select(pitch => new Note(duration, pitch)));
                var arpeggio = chord.Select(pitch => new Note(duration, pitch));

                start = end;
            }

            // First render the arpeggio notes
            foreach(var e in (new Serial(arpeggios.ToArray())).Render(parameters))
                yield return e;

            // Then render the other notes
            foreach (var e in voices.Render(parameters))
                yield return e;
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
