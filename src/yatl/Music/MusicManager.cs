using System;
using System.Collections.Generic;
using System.Linq;
using amulware.Graphics;
using Cireon.Audio;
using yatl.Utilities;

namespace yatl
{
    sealed class MusicManager
    {
        double time = 0;
        LinkedList<SoundEvent> eventSchedule = new LinkedList<SoundEvent>();
        BranchingMusicalComposition composition;
        Motif currentMotif;
        public SoundFile PianoSound;

        public MusicManager()
        {
            AudioManager.Initialize();
            this.PianoSound = new SoundFile("data/music/PianoC3.ogg");

            string filename = "data/music/foo.bmc";
            Console.WriteLine("Parsing " + filename);
            this.composition = new BranchingMusicalComposition(filename);
            this.currentMotif = this.composition.Root;
            this.Schedule(this.currentMotif.Render());
        }

        public void Schedule(IEnumerable<SoundEvent> soundEvents)
        {
            soundEvents = soundEvents.OrderBy(o => o.StartTime);
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
            Motif nextMotif = this.currentMotif.Successors.RandomElement();
            this.Schedule(nextMotif.Render());
            this.currentMotif = nextMotif;
        }

        public void Update(UpdateEventArgs args)
        {
            this.time += args.ElapsedTimeInS;

            while (this.eventSchedule.Count != 0 && this.eventSchedule.First.Value.StartTime <= this.time) {
                var nextEvent = this.eventSchedule.First.Value;
                this.eventSchedule.RemoveFirst();
                nextEvent.Execute(this);
            }

            if (this.eventSchedule.Count == 0)
                this.scheduleNextMotif();
            else {
                SoundEvent last = this.eventSchedule.Last.Value;
                double endOfMotif = last.StartTime;
                // If current motif ends in less than 5 seconds
                if (endOfMotif + 5 > this.time)
                    this.scheduleNextMotif();
            }
        }
    }
}
