using System;
using TK = OpenTK.Configuration;
using System.Globalization;
using System.Threading;

namespace yatl
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Everything returns false on linux ??
            foreach(bool config in new bool[]{
                    TK.RunningOnAndroid,
                    TK.RunningOnLinux,
                    TK.RunningOnMacOS,
                    TK.RunningOnMono,
                    TK.RunningOnSdl2,
                    TK.RunningOnUnix,
                    TK.RunningOnWindows,
                    TK.RunningOnX11,
                    }){
                Console.WriteLine(config.ToString());
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            System.Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            int glMajor = 1;
            int glMinor = 0;
            //int glMajor = OpenTK.Configuration.RunningOnLinux ? 1 : 3;
            //int glMinor = OpenTK.Configuration.RunningOnLinux ? 0 : 2;
            new YATLWindow(glMajor, glMinor).Run(Settings.General.DefaultFPS);
        }
    }
}
