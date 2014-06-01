using System;
using System.Collections.Generic;
using System.Linq;
using amulware.Graphics;
using Cireon.Audio;
using yatl.Environment;
using yatl.Utilities;

/* TODO
 * 
 * Should have:
 * Several types of speedups and slowdowns
 * Manually implement looping of buffers for ASR sounds
 * Several instruments for single note
 * More instrument samples
 * More frequencies in table
 * 
 * */

namespace yatl
{
    sealed class MusicManager
    {
        double time = 0;
        double speed = 1;
        LinkedList<SoundEvent> eventSchedule = new LinkedList<SoundEvent>();
        BranchingMusicalComposition composition;
        Motif currentMotif;
        Random random;

        public Instrument Piano;
        public Instrument Violin;
        OggStream ambient;

        public MusicParameters Parameters { get; set; }

        public MusicManager()
        {
            this.random = new Random();

            AudioManager.Initialize();
            this.ambient = new OggStream("data/music/ambient1.ogg");
            this.Piano = new SimpleInstrument("data/music/Piano.pp.C4_2.ogg", 261.6);
            this.Violin = new ASRInstrument("data/music/ViolinGis3-loop.ogg", "data/music/ViolinGis3-loop.ogg", "data/music/ViolinGis3-loop.ogg", 207.7);

            string filename = "data/music/foo.bmc";
            Console.WriteLine("Parsing " + filename);
            this.composition = new BranchingMusicalComposition(filename);
            this.currentMotif = this.composition.Root;

            this.Schedule(this.currentMotif.Render(0.5, this.Piano));

            this.ambient.IsLooped = true;
            this.ambient.Play();
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

        void scheduleNextMotif(double tension, double lightness)
        {
            //string tag = new string[] { "light", "dark" }.RandomElement();
            string tag = lightness > .5 ? "light" : "dark";
            Motif nextMotif = this.currentMotif.Successors.Where(o => o.Name.Contains(tag)).RandomElement();


            this.Schedule(nextMotif.Render(tension, this.Piano));
            this.currentMotif = nextMotif;
        }

        public void Update(UpdateEventArgs args)
        {
            double tension = this.Parameters.Tension;
            double lightness = this.Parameters.Lightness;
            this.speed = .8;

            double elapsedTime = args.ElapsedTimeInS * this.speed;
            this.time += elapsedTime;

            this.ambient.Volume = (float)(tension / 4 * (1 - lightness));

            // Play soundevents
            while (this.eventSchedule.Count != 0 && this.eventSchedule.First.Value.StartTime <= this.time) {
                var nextEvent = this.eventSchedule.First.Value;
                this.eventSchedule.RemoveFirst();
                nextEvent.Execute();
            }

            // Schedule soundevents
            if (this.eventSchedule.Count == 0)
                this.scheduleNextMotif(tension, lightness);
            else {
                // If current motif ends in less than 5 seconds, schedule next motif
                if (this.eventSchedule.Last.Value.StartTime - 5 < this.time)
                    this.scheduleNextMotif(tension, lightness);
            }

            AudioManager.Instance.Update((float)elapsedTime);
        }
    }
}
