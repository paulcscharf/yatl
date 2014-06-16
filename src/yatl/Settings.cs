using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace yatl
{
    static class Settings
    {
        public static class General
        {
            public const int DefaultWindowWidth = 1280;
            public const int DefaultWindowHeight = 720;
            public const int UpdateFPS = 50;
            public const int DrawFPS = 50;
        }

        public static class Game
        {
            public static class Wisp
            {
                public const float FrictionCoefficient = 3f;
                public const float Acceleration = 25f;
            }

            public static class Enemy
            {
                public const float FrictionCoefficient = 1.5f;
                public const float Acceleration = 17f;

                public const float ViewDistance = 20;

                public const float ViewDistanceSquared = ViewDistance * ViewDistance;

                public const float MinScanInterval = 0.1f;
                public const float MaxScanInterval = 0.3f;
            }

            public static class Camera
            {
                public readonly static Vector3 FocusOffset = new Vector3(0, -2, 0);
                public readonly static Vector3 PositionOffset = new Vector3(0, -10, 25);

                public const float DefaultZoom = 1;

                public const float OverviewZoom = 12f;

                public const float FocusForce = 5f;
                public const float PositionForce = 3f;
            }

            public static class Level
            {
                public const int Radius = 20;

                private const float sqrtOfThree = 1.73205080757f;

                public const float HexagonSide = 8;
                public const float HexagonDiameter = HexagonSide * 2;
                public const float HexagonWidth = HexagonSide * sqrtOfThree;

                public const float HexagonHeight = HexagonSide * 1.5f;

                public readonly static Vector2 HexagonGridUnitX = new Vector2(HexagonWidth, 0);
                public readonly static Vector2 HexagonGridUnitY = new Vector2(HexagonWidth * 0.5f, HexagonHeight);

                public readonly static amulware.Graphics.Matrix2 TileToPosition =
                    new amulware.Graphics.Matrix2(HexagonGridUnitX, HexagonGridUnitY);

                public const float HexagonInnerRadiusSquared = (HexagonWidth * 0.5f) * (HexagonWidth * 0.5f);
                public const float HexagonOuterRadiusSquared = HexagonSide * HexagonSide;

                public const float OverlayHeight = 2;

                public const float WallHeight = 2;
            }
        }

        public static class Content
        {
            public static class Shaders
            {
                public const string VertexShaderExtension = ".vs";
                public const string FragmentShaderExtension = ".fs";
                
                public const string ShaderRefreshPathPrefix = "../../";

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
