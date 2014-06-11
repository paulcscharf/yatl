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

        public override IEnumerable<SoundEvent> Render(RenderParameters parameters, double start = 0)
        {
            // First generate arpeggios
            // Then render the other notes
            //return voices.Render(parameters);
            foreach (var e in this.GenerateArpeggios(parameters).Concat(voices.Render(parameters, start)))
                yield return e;
        }

        public IEnumerable<SoundEvent> GenerateArpeggios(RenderParameters parameters)
        {
            double start = 0;
            foreach (var basenote in this.voices.Content[1].Content) {
                double end = start + basenote.Duration;
                yield return new StartRubato(start); // Rubato for each chord
                yield return new LiftSustain(start); // Lift sustain before each chord

                // Gather arpeggio set
                var arpeggio = new List<Note>();
                var arpeggioSpace = new List<Pitch>();
                foreach (var voice in this.voices.Content) {
                    //foreach (var pitch in voice.GetRange(start, end).Select(note => note.Pitch)) {
                    foreach (var pitch in voice.GetPosition(start).Select(note => note.Pitch)) {
                        arpeggioSpace.Add(pitch);
                        arpeggioSpace.Add(pitch.NextOctave());
                        arpeggioSpace.Add(pitch.PreviousOctave());
                    }
                }

                // Select tones
                var up = arpeggioSpace.SelectRandom(6).OrderBy(o => o.Frequency).ToList();
                var down = arpeggioSpace.SelectRandom(6).OrderByDescending(o => o.Frequency).ToList();
                double duration = basenote.Duration / (double) (up.Count + down.Count);
                arpeggio.AddRange(up.Select(pitch => new Note(duration, pitch)));
                arpeggio.AddRange(down.Select(pitch => new Note(duration, pitch)));

                foreach(var e in (new Serial(arpeggio.ToArray())).Render(parameters, start))
                    yield return e;

                start = end;
            }
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
