//AudioManager.Initialize();
//Console.WriteLine(AudioManager.Instance.MasterVolume);
//AudioManager.Instance.MasterVolume = 1.9f;
//Console.WriteLine(AudioManager.Instance.MasterVolume);

//PianoSound = new SoundFile("data/music/PianoC3.ogg");
//var s1 = PianoSound.GenerateSource();
//var s2 = PianoSound.GenerateSource();
//var s3 = PianoSound.GenerateSource();
//s1.Volume = 1.9f;
//s1.Play();
//s2.Pitch = 2;
//s2.Play();
//System.Threading.Thread.Sleep(1000);
//s3.Pitch = 3f;
//s3.Play();
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
        SoundFile pianoSound = new SoundFile("data/music/PianoC3.ogg");
        private Queue<SoundEvent> scheduledEvents = new Queue<SoundEvent>();


        public MusicManager()
        {
            AudioManager.Initialize();
			var s1 = pianoSound.GenerateSource();
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


            // random notes shenanigans
            //if (this.time >= this.nextNoteTime)
            //{
            //    var source = this.pianoSound.GenerateSource();
            //    source.Pitch = GlobalRandom.NextFloat(1f, 3f);
            //    source.Play();

            //    this.nextNoteTime = this.time + GlobalRandom.NextFloat(0.2f, 2f);
            //}
        }
    }
}
