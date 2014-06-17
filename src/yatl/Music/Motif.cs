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
                int density = (int) (2 * parameters.Density) + 1;
                Console.WriteLine("density: " + density.ToString());
                //int[] pattern = new int[] { 1, 0 }; // 1 = up, 0 = down
                var pattern = Enumerable.Range(0, density).Select(i => GlobalRandom.Next(2));
                Console.WriteLine("pattern: " + string.Join(", ", pattern.Select(o=>o.ToString())));
                //var pattern = Enumerable.Range(0, 1).SelectRandom(2);

                foreach(var direction in pattern)
                {
                    int numberOfTones = Enumerable.Range(2, 2).RandomElement() * density;
                    double duration = basenote.Duration / (double) (numberOfTones * pattern.Count());

                    List<Pitch> stroke;
                    if (direction == 0) 
                        stroke = arpeggioSpace.SelectRandom(numberOfTones).OrderByDescending(o => o.Frequency).ToList();
                    else
                        stroke = arpeggioSpace.SelectRandom(numberOfTones).OrderBy(o => o.Frequency).ToList();
                    arpeggio.AddRange(stroke.Select(pitch => new Note(duration, pitch)));
                    // Hack to prevent double notes at the end of a stroke
                    //arpeggioSpace.Remove(stroke.Last());
                    //arpeggioSpace.Add(stroke.First());
                }

                foreach(var e in (new Serial(arpeggio.ToArray())).Render(parameters, start))
                    yield return e;

                start = end;
                yield return new StartRubato(start - 0.1); // Some articulation just before the next chord
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
