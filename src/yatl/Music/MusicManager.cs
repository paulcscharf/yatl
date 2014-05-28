using System;
using System.Collections.Generic;
using amulware.Graphics;
using Cireon.Audio;
using yatl.Utilities;

namespace yatl
{
    sealed class MusicManager
    {
        private double time = 0;
        SoundFile pianoSound;
        private Queue<SoundEvent> scheduledEvents = new Queue<SoundEvent>();


        public MusicManager()
        {
            AudioManager.Initialize();
            this.pianoSound = new SoundFile("data/music/PianoC3.ogg");
			var s1 = this.pianoSound.GenerateSource();
			s1.Play();
            System.Threading.Thread.Sleep(1000);

            string filename = "data/music/foo.bmc";
            Console.WriteLine("Parsing " + filename);
            var composition = BranchingMusicalComposition.FromFile(filename);

            this.Schedule(composition.root.Render());
        }

        public void Schedule(IEnumerable<SoundEvent> soundEvents)
        {
            foreach (var soundEvent in soundEvents) {
                soundEvent.AddOffset(this.time);
                this.scheduledEvents.Enqueue(soundEvent);
            }
        }

        void playNote(Note note)
        {
            var source = this.pianoSound.GenerateSource();
            source.Pitch = (float) (note.Frequency / 130.8);
            source.Play();
            Console.WriteLine("Playing note " + note.ToString());
            Console.WriteLine("Pitch " + source.Pitch.ToString());
            Console.WriteLine("Time " + this.time.ToString());
        }

        public void Update(UpdateEventArgs args)
        {
            this.time += args.ElapsedTimeInS;

            while (this.scheduledEvents.Count != 0 && this.scheduledEvents.Peek().StartTime <= this.time) {
                var next = this.scheduledEvents.Dequeue();
                this.playNote(next.Note);
            }
        }
    }
}
