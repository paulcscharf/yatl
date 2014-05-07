using System;
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
    static class GraphicsHelpers
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

        public static ShaderProgram LoadShaderProgram(string path)
        {
            return GraphicsHelpers.LoadShaderProgram(
                path + Settings.Content.Shaders.VertexShaderExtension,
                path + Settings.Content.Shaders.FragmentShaderExtension
                );
        }

        public static ShaderProgram LoadShaderProgram(string vspath, string fspath)
        {
            using (var vsReader = new StreamReader(vspath))
            using (var fsReader = new StreamReader(fspath))
            {
                return ShaderProgram.FromCode(
                    Settings.Content.Shaders.ShaderCodePrefix + vsReader.ReadToEnd(),
                    Settings.Content.Shaders.ShaderCodePrefix + fsReader.ReadToEnd()
                    );   
            }
        }

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
