using System;
using amulware.Graphics;
using Cireon.Audio;
using yatl.Utilities;

namespace yatl
{
    sealed class MusicManager
    {
        private double time = 0;


        private SoundFile sf;
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

            this.sf = new SoundFile("data/music/Piano.ogg");

            this.nextNoteTime = 0;
        }

        public void Update(UpdateEventArgs args)
        {
            this.time += args.ElapsedTimeInS;


            // random notes shenanigans
            if (this.time >= this.nextNoteTime)
            {
                var source = this.sf.GenerateSource();
                source.Pitch = GlobalRandom.NextFloat(1f, 3f);
                source.Play();

                this.nextNoteTime = this.time + GlobalRandom.NextFloat(0.2f, 2f);
            }
        }
    }
}
