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
        Queue<SoundEvent> eventSchedule = new Queue<SoundEvent>();
        public SoundFile PianoSound;

        public MusicManager()
        {
            AudioManager.Initialize();
            this.PianoSound = new SoundFile("data/music/PianoC3.ogg");

            string filename = "data/music/foo.bmc";
            Console.WriteLine("Parsing " + filename);
            var composition = BranchingMusicalComposition.FromFile(filename);

            this.Schedule(composition.root.Render());
        }

        public void Schedule(IEnumerable<SoundEvent> soundEvents)
        {
            soundEvents = soundEvents.OrderBy(o => o.StartTime);

            foreach (var soundEvent in soundEvents) {
                soundEvent.AddOffset(this.time);
                this.eventSchedule.Enqueue(soundEvent);
            }
        }

        public void Update(UpdateEventArgs args)
        {
            this.time += args.ElapsedTimeInS;

            while (this.eventSchedule.Count != 0 && this.eventSchedule.Peek().StartTime <= this.time) {
                var nextEvent = this.eventSchedule.Dequeue();
                nextEvent.Execute(this);
            }
        }
    }
}
