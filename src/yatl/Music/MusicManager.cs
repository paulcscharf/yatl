using System;
using System.Collections.Generic;
using System.Linq;
using amulware.Graphics;
using Cireon.Audio;
using yatl.Environment;
using yatl.Utilities;

/* 
 * SHOULD HAVE
 * Several types of speedups and slowdowns
 * Broken chords
 * More frequencies in table
 * Automatically add octaves to melody or base
 * Automatically add more tones on the chord (with arpeggio's)
 * "Opbouw" when staying in a subgraph
 * Automatic asyncopes
 * 
 * COULD HAVE
 * More instrument samples
 * Several instruments for single note
 * 
 * NOTES
 * We may want to have a seperate loop for generation, for the delay it causes in playing music
 * Why not pass MusicParameters to Update method?
 * Implementing looping of buffers for ASR sounds is not reasonably doable
 * 
 * IDEAS
 * arpeggio algorithm: generate an arpeggio set from octaves, sort by frequency, schedule them according to some pattern
 * need to specify octaves, arpeggio density and arpeggio pattern (in terms of up/down, i.e. a bitstring)
 * 
 * rubato algorithm: start of measure must be slow and soft, speeding up linearly till the end, then fallback abruptly
 * could be implemented by introducing rubato soundevents
 * need to specify volume and speed interval
 * 
 * */

namespace yatl
{
    sealed class MusicManager
    {
        double time = 0;

        LinkedList<SoundEvent> eventSchedule = new LinkedList<SoundEvent>();
        BranchingMusicalComposition composition;
        Motif currentMotif;

        public static Random Random = new Random();
        public static List<Sound> SustainSet = new List<Sound>();
        public static double Speed = 1;
        public static double Acceleration = 1;
        public static double Volume = 1;

        public Instrument Piano;
        public Instrument Violin;
        public Instrument Strings;
        OggStream ambient;

        public MusicParameters Parameters { get; set; }

        public MusicManager()
        {
            AudioManager.Initialize();
            this.ambient = new OggStream("data/music/ambient1.ogg");
            //this.Piano = new SimpleInstrument("data/music/Piano.pp.C4_2.ogg", 261.6);
            this.Piano = new Piano();
            this.Violin = new SRInstrument("data/music/ViolinGis3-loop.ogg", "data/music/ViolinGis3-decay.ogg", 207.7);
            //this.Strings = new SimpleInstrument("data/music/StringsC5.ogg", 523.3);
            this.Strings = new SRInstrument("data/music/StringsC5-sustain.ogg", "data/music/StringsC5-decay.ogg", 523.3);

            string filename = "data/music/DarkAndLight.bmc";
            Console.WriteLine("Parsing " + filename);
            this.composition = new BranchingMusicalComposition(filename);

            this.ambient.IsLooped = true;
            //this.ambient.Play();

            this.Parameters = new MusicParameters(0.5f, 0.5f);
        }

        public void Schedule(IEnumerable<SoundEvent> soundEvents)
        {
            soundEvents = soundEvents.OrderBy(o => o.StartTime);

            // Place the soundevents after the last event that is currently scheduled
            double offset;
            if (this.eventSchedule.Count == 0)
                offset = this.time;
            else
                offset = this.eventSchedule.Last.Value.StartTime;

            foreach (var soundEvent in soundEvents) {
                soundEvent.AddOffset(offset);
                this.eventSchedule.AddLast(soundEvent);
            }
        }

        void scheduleNextMotif()
        {
            if (this.currentMotif == null)
                this.currentMotif = this.composition.Root;
            else {
                string tag = this.Parameters.Lightness > .5 ? "light" : "dark";
                if (this.Parameters.Lightness > .5) {
                    tag = "light";
                    if (this.currentMotif.Name.Contains("dark"))
                        tag = "dusk";
                }
                else {
                    tag = "dark";
                    if (this.currentMotif.Name.Contains("light"))
                        tag = "dusk";
                }
                this.currentMotif = this.currentMotif.Successors.Where(o => o.Name.Contains(tag)).RandomElement();
            }

            RenderParameters parameters = new RenderParameters(this.Parameters, this.Piano, 1);
            parameters.Density = 1;
            this.Schedule(this.currentMotif.Render(parameters));
        }

        public void Update(UpdateEventArgs args)
        {
            Speed = Math.Min(1.2, Speed + args.ElapsedTimeInS * Acceleration);
            Volume = 2 * (1.4 - Speed);
            double elapsedTime = args.ElapsedTimeInS * Speed;
            this.time += elapsedTime;
            //Speed = Math.Sin(time) * Math.Sin(time) + .5;

            double tension = this.Parameters.Tension;
            double lightness = this.Parameters.Lightness;


            this.ambient.Volume = (float)(.5 * tension * (1 - lightness));

            // Play soundevents
            while (this.eventSchedule.Count != 0 && this.eventSchedule.First.Value.StartTime <= this.time) {
                var nextEvent = this.eventSchedule.First.Value;
                this.eventSchedule.RemoveFirst();
                nextEvent.Execute();
                Console.WriteLine(nextEvent);
            }

            // Schedule soundevents
            if (this.eventSchedule.Count == 0)
                this.scheduleNextMotif();
            else {
                // If current motif ends in less than 1 seconds, schedule next motif
                if (this.eventSchedule.Last.Value.StartTime - 1 < this.time)
                    this.scheduleNextMotif();
            }

            AudioManager.Instance.Update((float)elapsedTime);
        }
    }
}
