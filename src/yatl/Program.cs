
namespace yatl
{
    static class Program
    {
        static void Main(string[] args)
        {
            int glMajor = OpenTK.Configuration.RunningOnLinux ? 1 : 3;
            int glMinor = OpenTK.Configuration.RunningOnLinux ? 0 : 2;
            new YATLWindow(glMajor, glMinor).Run(Settings.General.DefaultFPS);
        }
    }
}
