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

             var sf = new SoundFile("data/music/Piano.ogg");
             var source = sf.GenerateSource();
             //source.Volume = 2;
             source.Play();
             System.Threading.Thread.Sleep(1000);
             source.Pitch = 2;
             source.Play();
             System.Threading.Thread.Sleep(1000);
             source.Pitch = 3;
             source.Play();
         }
    }
}
