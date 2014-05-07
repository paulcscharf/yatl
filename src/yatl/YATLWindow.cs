using System;
using amulware.Graphics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using yatl.Utilities;

namespace yatl
{
    sealed class YATLWindow : amulware.Graphics.Program
    {
        public YATLWindow()
            : base(Settings.General.DefaultWindowWidth,
                Settings.General.DefaultWindowHeight,
                GraphicsMode.Default, "You are the light",
                GameWindowFlags.Default, DisplayDevice.Default, 3, 2,
                GraphicsContextFlags.Default)
        {
            Console.WriteLine(GL.GetString(StringName.Version));
            Console.WriteLine(GL.GetString(StringName.ShadingLanguageVersion));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InputManager.Initialize(this.Mouse);


        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            if(this.Keyboard[Key.Escape])
                this.Close();
        }

        protected override void OnRender(UpdateEventArgs e)
        {

        }
    }
}
