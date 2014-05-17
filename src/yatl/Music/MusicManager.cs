using System;

namespace yatl
{
    sealed class MusicManager
    {
         public MusicManager()
         {
             string filename = "data/music/foo.bmc";
             Console.WriteLine("Parsing " + filename);
             BranchingMusicalComposition.FromFile(filename);
         }
    }
}
