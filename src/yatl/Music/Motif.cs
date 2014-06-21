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
                var arpeggioSpace = new List<Pitch>();
                foreach (var voice in this.voices.Content) {
                    foreach (var pitch in voice.GetPosition(start).Select(note => note.Pitch)) {
                        if (pitch.Frequency > 0) {
                            arpeggioSpace.Add(pitch);
                            arpeggioSpace.Add(pitch.NextOctave());
                            arpeggioSpace.Add(pitch.PreviousOctave());
                        }
                    }
                }
                arpeggioSpace = arpeggioSpace.OrderBy(o => o.Frequency).ToList();

                int density = (int)Math.Round(2 * parameters.MusicParameters.Tension + 1);

                IEnumerable<int> pattern;
                int tonesPerChord;
                if (density == 1) {
                    pattern = new int[] { 1 };
                    tonesPerChord = 3;
                }
                else if (density == 2) {
                    pattern = new int[] { 1, -1 };
                    tonesPerChord = 6;
                }
                else {
                    pattern = new int[] { 1, -1, 1 };
                    tonesPerChord = 12;
                }
                int tonesPerStroke = tonesPerChord / pattern.Count();
                double duration = basenote.Duration / (double)(tonesPerChord);

                Console.WriteLine("pattern: " + string.Join(", ", pattern.Select(o => o.ToString())));
                Console.WriteLine("duration: " + duration.ToString());

                // Select tones
                IEnumerable<Note> arpeggio = this.SelectArpeggioSequence(arpeggioSpace, pattern, tonesPerStroke, duration);

                // Render and yield arpeggio
                foreach (var e in (new Serial(arpeggio.ToArray())).Render(parameters, start))
                    yield return e;

                start = end;
                yield return new StartRubato(start - 0.1); // Some articulation just before the next chord
            }
        }

        public IEnumerable<Note> SelectArpeggioSequence(List<Pitch> arpeggioSpace, IEnumerable<int> pattern, int tonesPerStroke, double duration)
        {
            int index = 0;

            foreach (var direction in pattern) {
                for (int i = 0; i < tonesPerStroke; i++) {
                    yield return new Note(duration, arpeggioSpace[index]);
                    index += direction;

                    // With a certain probability, skip a tone
                    int tonesToGenerate = tonesPerStroke - (i + 1);
                    int remainingArpeggioSpace = direction == 1 ? arpeggioSpace.Count - index : index;
                    int slack = remainingArpeggioSpace - tonesToGenerate;
                    double pSkip = (double)slack / (double)remainingArpeggioSpace;
                    if (MusicManager.Random.NextDouble() < pSkip)
                        index += direction;
                }
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
