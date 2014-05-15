using System;
using System.Globalization;
using System.Threading;

namespace yatl
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(OpenTK.Configuration.RunningOnLinux.ToString()); // Returns False!!

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
