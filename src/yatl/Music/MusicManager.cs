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
//System.Threading.Thread.Sleep(1000);
//s2.Pitch = 2;
//s2.Play();
//System.Threading.Thread.Sleep(1000);
//s3.Pitch = 3f;
//s3.Play();
using System;
using amulware.Graphics;
using Cireon.Audio;
using yatl.Utilities;

namespace yatl
{
    sealed class MusicManager
    {
        private double time = 0;


        SoundFile pianoSound = new SoundFile("data/music/PianoC3.ogg");
        private double nextNoteTime;


        public MusicManager()
        {
            string filename = "data/music/foo.bmc";
            Console.WriteLine("Parsing " + filename);
            BranchingMusicalComposition.FromFile(filename);

            AudioManager.Initialize();
            Console.WriteLine(AudioManager.Instance.MasterVolume);
            //AudioManager.Instance.MasterVolume = 2;
            Console.WriteLine(AudioManager.Instance.MasterVolume);

            this.nextNoteTime = 0;
        }

        public void PlayNote(Note note)
        {

            var source = this.pianoSound.GenerateSource();
            //source.Pitch = (float) (note.pitch.Frequency / 130.8);
            source.Play();
        }

        public void Update(UpdateEventArgs args)
        {
            this.time += args.ElapsedTimeInS;


            // random notes shenanigans
            if (this.time >= this.nextNoteTime)
            {
                var source = this.pianoSound.GenerateSource();
                source.Pitch = GlobalRandom.NextFloat(1f, 3f);
                source.Play();

                this.nextNoteTime = this.time + GlobalRandom.NextFloat(0.2f, 2f);
            }
        }
    }
}
