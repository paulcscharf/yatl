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
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            System.Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            int glMajor = 1;
            int glMinor = 0;
            new YATLWindow(glMajor, glMinor).Run(Settings.General.DefaultFPS);
        }
    }
}
