using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using amulware.Graphics;
using OpenTK.Graphics.OpenGL;
using System.IO;
using OpenTK.Input;
using Encoder = System.Drawing.Imaging.Encoder;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace yatl.Utilities
{
    static class GraphicsHelper
    {
        public static VertexShader LoadVertexShader(string path)
        {
            using (var reader = new StreamReader(path))
            {
                return new VertexShader(Settings.Content.Shaders.ShaderCodePrefix + reader.ReadToEnd());
            }

        }

        public static FragmentShader LoadFragmentShader(string path)
        {
            using (var reader = new StreamReader(path))
            {
                return new FragmentShader(Settings.Content.Shaders.ShaderCodePrefix + reader.ReadToEnd());
            }
        }

        public static ISurfaceShader LoadShaderProgram(string path)
        {
            return GraphicsHelper.LoadShaderProgram(
                path + Settings.Content.Shaders.VertexShaderExtension,
                path + Settings.Content.Shaders.FragmentShaderExtension
                );
        }

        public static ISurfaceShader LoadShaderProgram(string vspath, string fspath)
        {
            var program = GraphicsHelper.loadShaderProgram(vspath, fspath);
#if DEBUG
            var refresher = new ShaderProgramRefresher(program);

            GraphicsHelper.subscribeToShaderChanges(vspath, fspath, refresher);

            return refresher;
#endif
            return program;
        }

        private static ShaderProgram loadShaderProgram(string vspath, string fspath)
        {
            using (var vsReader = new StreamReader(vspath))
            using (var fsReader = new StreamReader(fspath))
            {
                var program = ShaderProgram.FromCode(
                    Settings.Content.Shaders.ShaderCodePrefix + vsReader.ReadToEnd(),
                    Settings.Content.Shaders.ShaderCodePrefix + fsReader.ReadToEnd()
                    );
                return program;
            }
        }

#if DEBUG
        private static readonly List<ShaderRefreshContainer> refreshableShaders =
            new List<ShaderRefreshContainer>();

        private sealed class ShaderRefreshContainer
        {
            private readonly ShaderProgramRefresher refresher;
            private readonly FileRefreshInfo vs;
            private readonly FileRefreshInfo fs;

            public ShaderRefreshContainer(string vspath, string fspath, ShaderProgramRefresher refresher)
            {
                this.vs = new FileRefreshInfo(Settings.Content.Shaders.ShaderRefreshPathPrefix + vspath);
                this.fs = new FileRefreshInfo(Settings.Content.Shaders.ShaderRefreshPathPrefix + fspath);
                this.refresher = refresher;
            }

            public void RefreshIfModified(out bool changed, out bool reloaded)
            {
                changed = false;
                reloaded = false;

                if (this.fs.FileName == "beard.fs")
                {

                }

                bool vChanged = this.vs.WasModified();
                bool fChanged = this.fs.WasModified();

                if (!vChanged && !fChanged)
                    return;

                changed = true;

                string changedName = (vChanged && fChanged)
                    ? string.Format("{0} and {1}", this.vs.FileName, this.fs.FileName)
                    : (vChanged ? this.vs.FileName : this.fs.FileName);

                var argb = Console.ForegroundColor;

                Console.WriteLine("\n+++++++++++++++++++++");
                Console.WriteLine("{0} changed.", changedName);
                Console.WriteLine("Reloading {0} and {1} ..", this.vs.FileName, this.fs.FileName);

                var watch = Stopwatch.StartNew();

                ShaderProgram program;

                try
                {
                    program = GraphicsHelper.loadShaderProgram(this.vs.Path, this.fs.Path);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Could not compile shader! (took {0:0.00}ms)", watch.Elapsed.TotalMilliseconds);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(e.Message);
                    program = null;
                }

                if (program != null)
                {
                    this.refresher.SetShaderProgram(program);

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Shader successfully reloaded! (took {0:0.00}ms)", watch.Elapsed.TotalMilliseconds);

                    reloaded = true;
                }
                Console.WriteLine();

                Console.ForegroundColor = argb;
            }
        }

        private static void subscribeToShaderChanges(string vspath, string fspath, ShaderProgramRefresher refresher)
        {
            GraphicsHelper.refreshableShaders.Add(new ShaderRefreshContainer(vspath, fspath, refresher));
        }

        public static void CheckAndUpdateChangedShaders()
        {
            var watch = Stopwatch.StartNew();
            int changeCounter = 0;
            int reloadCounter = 0;
            foreach (var refresher in GraphicsHelper.refreshableShaders)
            {
                bool changed;
                bool reloaded;
                refresher.RefreshIfModified(out changed, out reloaded);
                if (changed)
                    changeCounter++;
                if (reloaded)
                    reloadCounter++;
            }
            watch.Stop();

            if (changeCounter != 0)
            {
                var argb = Console.ForegroundColor;

                Console.WriteLine("Checked {0} shaders for changes (took {1:0.00}ms)",
                    GraphicsHelper.refreshableShaders.Count, watch.Elapsed.TotalMilliseconds);
                Console.WriteLine("{0} where modified", changeCounter);
                Console.ForegroundColor = changeCounter == reloadCounter
                    ? ConsoleColor.DarkGreen
                    : (reloadCounter == 0 ? ConsoleColor.Red : ConsoleColor.Yellow);
                Console.WriteLine("{0} where reloaded successfully", reloadCounter);

                Console.ForegroundColor = argb;
            }
        }
#endif

        // Returns a System.Drawing.Bitmap with the contents of the current framebuffer
        public static Bitmap GrabScreenshot(this amulware.Graphics.Program program)
        {
            int width = program.ClientSize.Width;
            int height = program.ClientSize.Height;

            float ratio = (float)width / height;

            int x = 0, y = 0, w = width, h = height;

            if (ratio >= 16.0 / 9.0)
            {
                w = (int)(height * 16.0 / 9.0);
                x = (width - w) / 2;
            }
            else
            {
                h = (int)(width * 9.0 / 16.0);
                y = (height - h) / 2;
            }

            var bmp = new Bitmap(w, h);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h), 
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(x, y, w, h, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }

        public static void MakeScreenshot(this amulware.Graphics.Program program)
        {
            using (var bitmap = program.GrabScreenshot())
            {
                if (!Directory.Exists(Settings.ScreenShot.Path))
                    Directory.CreateDirectory(Settings.ScreenShot.Path);

                string filename = Settings.ScreenShot.Path + "rf_" + DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss.ff");

                if (Settings.ScreenShot.SaveAsPng)
                {
                    bitmap.Save(filename + ".png", System.Drawing.Imaging.ImageFormat.Png);   
                }
                else
                {
                    var encoder = ImageCodecInfo.GetImageDecoders()
                        .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
                    var parameters = new EncoderParameters(1);
                    parameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                    bitmap.Save(filename + ".jpg", encoder, parameters);
                }

                Console.WriteLine("Screenshot saved to {0}", filename);
            }
        }

        public static string ToButtonString(this Key key)
        {
            string k;
            switch (key)
            {
                case Key.AltLeft:
                    k = "Left Alt";
                    break;
                case Key.AltRight:
                    k = "Right Alt";
                    break;
                case Key.BackSpace:
                    k = "<--"; // replace with symbol
                    break;
                case Key.BackSlash:
                    k = " \\ ";
                    break;
                case Key.BracketLeft:
                    k = " [ ";
                    break;
                case Key.BracketRight:
                    k = " ] ";
                    break;
                case Key.CapsLock:
                    k = "Caps Lock"; // replace with symbol
                    break;
                case Key.Comma:
                    k = " , ";
                    break;
                case Key.ControlLeft:
                    k = "Left Ctrl";
                    break;
                case Key.ControlRight:
                    k = "Left Ctrl";
                    break;
                case Key.Delete:
                    k = "Del";
                    break;
                case Key.Down:
                    k = Settings.Font.Characters.ArrowDown.ToString();
                    break;
                case Key.End:
                    k = "End";
                    break;
                case Key.Enter:
                    k = "Enter"; // replace with symbol
                    break;
                case Key.Escape:
                    k = "Esc";
                    break;
                case Key.Insert:
                    k = "Ins";
                    break;
                case Key.Keypad0:
                    k = "Num 0";
                    break;
                case Key.Keypad1:
                    k = "Num 1";
                    break;
                case Key.Keypad2:
                    k = "Num 2";
                    break;
                case Key.Keypad3:
                    k = "Num 3";
                    break;
                case Key.Keypad4:
                    k = "Num 4";
                    break;
                case Key.Keypad5:
                    k = "Num 5";
                    break;
                case Key.Keypad6:
                    k = "Num 6";
                    break;
                case Key.Keypad7:
                    k = "Num 7";
                    break;
                case Key.Keypad8:
                    k = "Num 8";
                    break;
                case Key.Keypad9:
                    k = "Num 9";
                    break;
                case Key.KeypadAdd:
                    k = "Num +";
                    break;
                case Key.KeypadDecimal:
                    k = "Num .";
                    break;
                case Key.KeypadDivide:
                    k = "Num /";
                    break;
                case Key.KeypadEnter:
                    k = "Num Enter"; // replace with symbol?
                    break;
                case Key.KeypadMinus:
                    k = "Num -";
                    break;
                case Key.KeypadMultiply:
                    k = "Num *";
                    break;
                case Key.LastKey:
                    k = "Last Key";
                    break;
                case Key.Left:
                    k = Settings.Font.Characters.ArrowLeft.ToString();
                    break;
                case Key.Minus:
                    k = " - ";
                    break;
                case Key.NumLock:
                    k = "Num Lock";
                    break;
                case Key.Number0:
                    k = "0";
                    break;
                case Key.Number1:
                    k = "1";
                    break;
                case Key.Number2:
                    k = "2";
                    break;
                case Key.Number3:
                    k = "3";
                    break;
                case Key.Number4:
                    k = "4";
                    break;
                case Key.Number5:
                    k = "5";
                    break;
                case Key.Number6:
                    k = "6";
                    break;
                case Key.Number7:
                    k = "7";
                    break;
                case Key.Number8:
                    k = "8";
                    break;
                case Key.Number9:
                    k = "9";
                    break;
                case Key.PageDown:
                    k = "Page Down";
                    break;
                case Key.PageUp:
                    k = "Page Up";
                    break;
                case Key.Period:
                    k = " . ";
                    break;
                case Key.Plus:
                    k = " + ";
                    break;
                case Key.PrintScreen:
                    k = "Print";
                    break;
                case Key.Quote:
                    k = " ' ";
                    break;
                case Key.Right:
                    k = Settings.Font.Characters.ArrowRight.ToString();
                    break;
                case Key.ScrollLock:
                    k = "Scroll Lock";
                    break;
                case Key.Semicolon:
                    k = " ; ";
                    break;
                case Key.ShiftLeft:
                    k = "Left Shift";
                    break;
                case Key.ShiftRight:
                    k = "Right Shift";
                    break;
                case Key.Slash:
                    k = " / ";
                    break;
                case Key.Space:
                    k = "     Space     ";
                    break;
                case Key.Tab:
                    k = "Tab"; // replace with symbol?
                    break;
                case Key.Tilde:
                    k = " ~ ";
                    break;
                case Key.Unknown:
                    k = "Unknown";
                    break;
                case Key.Up:
                    k = Settings.Font.Characters.ArrowUp.ToString();
                    break;
                case Key.WinLeft:
                    k = "Left Win"; // replace with symbol?
                    break;
                case Key.WinRight:
                    k = "Right Win"; // replace with symbol?
                    break;
                default:
                    k = key.ToString();
                    break;
            }
            return "{" + k + "}";
        }

    }
}
