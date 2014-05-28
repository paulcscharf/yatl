using System;
using Cireon.Audio;

namespace yatl
{
    sealed class MusicManager
    {
         public MusicManager()
         {
             string filename = "data/music/foo.bmc";
             Console.WriteLine("Parsing " + filename);
             BranchingMusicalComposition.FromFile(filename);
             AudioManager.Initialize();
             Console.WriteLine(AudioManager.Instance.MasterVolume);
             //AudioManager.Instance.MasterVolume = 2;
             Console.WriteLine(AudioManager.Instance.MasterVolume);

             var sf = new SoundFile("data/music/PianoC3.ogg");
             var s1 = sf.GenerateSource();
             var s2 = sf.GenerateSource();
             var s3 = sf.GenerateSource();
             //source.Volume = 2;
             s1.Play();
             System.Threading.Thread.Sleep(1000);
             s2.Pitch = 2;
             s2.Play();
             System.Threading.Thread.Sleep(1000);
             s3.Pitch = 3;
             s3.Play();
         }
    }
}
