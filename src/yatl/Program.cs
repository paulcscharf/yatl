using System;

namespace yatl
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(OpenTK.Configuration.RunningOnLinux.ToString()); // Returns False!!
            int glMajor = 1;//OpenTK.Configuration.RunningOnLinux ? 1 : 3;
            int glMinor = 0;//OpenTK.Configuration.RunningOnLinux ? 0 : 2;
            new YATLWindow(glMajor, glMinor).Run(Settings.General.DefaultFPS);
        }
    }
}
