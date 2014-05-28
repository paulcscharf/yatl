using System;
using Cireon.Audio;

namespace yatl
{
    // TODO: maybe this class should be static
    sealed class MusicManager
    {
        public static SoundFile PianoSound;

        public MusicManager()
        {
            string filename = "data/music/foo.bmc";
            Console.WriteLine("Parsing " + filename);
            BranchingMusicalComposition.FromFile(filename);
            AudioManager.Initialize();
            Console.WriteLine(AudioManager.Instance.MasterVolume);
            AudioManager.Instance.MasterVolume = 1.9f;
            Console.WriteLine(AudioManager.Instance.MasterVolume);

            PianoSound = new SoundFile("data/music/PianoC3.ogg");
            var s1 = PianoSound.GenerateSource();
            var s2 = PianoSound.GenerateSource();
            var s3 = PianoSound.GenerateSource();
            s1.Volume = 1.9f;
            s1.Play();
            System.Threading.Thread.Sleep(1000);
            s2.Pitch = 2;
            s2.Play();
            System.Threading.Thread.Sleep(1000);
            s3.Pitch = 3f;
            s3.Play();
        }
    }
}
