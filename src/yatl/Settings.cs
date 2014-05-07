using OpenTK.Graphics.OpenGL;

namespace yatl
{
    static class Settings
    {
        public static class General
        {
            public const int DefaultWindowWidth = 1280;
            public const int DefaultWindowHeight = 720;
            public const int DefaultFPS = 50;
        }

        public static class Content
        {
            public static class Shaders
            {
                public const string VertexShaderExtension = ".vs";
                public const string FragmentShaderExtension = ".fs";

                public static string ShaderCodePrefix
                {
                    get
                    {
                        if (Settings.Content.Shaders.shaderCodePrefix == null)
                            Settings.Content.Shaders.initShaderCodePrefix();
                        return Settings.Content.Shaders.shaderCodePrefix;
                    }
                }

                private static string shaderCodePrefix;

                private static void initShaderCodePrefix()
                {
                    string shaderVersion = GL.GetString(StringName.ShadingLanguageVersion);
                    var ss = shaderVersion.Split(' ')[0].Split('.');
                    int sMajor = int.Parse(ss[0]);
                    int sMinor = int.Parse(ss[1]);

                    bool fallbackTo130 = sMajor == 1 && sMinor < 50;

                    Settings.Content.Shaders.shaderCodePrefix = string.Format("#version {0}\n",
                        fallbackTo130 ? "130" : "150");
                }
            }
        }

        public static class Input
        {
            public static class Gamepad
            {
                public static class Sticks
                {
                    public const float DeadZone = 0.01f;
                    public const float MaxValue = 0.95f;

                    public const float DeadToMaxRange =
                        Settings.Input.Gamepad.Sticks.MaxValue - Settings.Input.Gamepad.Sticks.DeadZone;

                    public const float HitValue = 0.6f;
                    public const float ReleaseValue = 0.4f;
                }
            }
        }

        public static class ScreenShot
        {
            public const bool SaveAsPng = false;
            public const string Path = "screenshots/";
        }

        public static class Font
        {
            public static class Characters
            {
                public const char ArrowLeft = (char)132;
                public const char ArrowUp = (char)133;
                public const char ArrowRight = (char)134;
                public const char ArrowDown = (char)135;
            }
        }
    }
}
