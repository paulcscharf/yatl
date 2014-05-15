using System;
using amulware.Graphics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using yatl.Environment;
using yatl.Rendering;
using yatl.Utilities;

namespace yatl
{
    sealed class YATLWindow : amulware.Graphics.Program
    {
        private GameRenderer renderer;
        private GameState gamestate;

        public YATLWindow(int glMajor, int glMinor)
            : base(Settings.General.DefaultWindowWidth,
                Settings.General.DefaultWindowHeight,
                GraphicsMode.Default, "You are the light",
                GameWindowFlags.Default, DisplayDevice.Default, glMajor, glMinor,
                GraphicsContextFlags.Default)
        {
            Console.WriteLine(GL.GetString(StringName.Version));
            Console.WriteLine(GL.GetString(StringName.ShadingLanguageVersion));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InputManager.Initialize(this.Mouse);

            this.renderer = new GameRenderer();

            this.gamestate = new GameState();
        }

        protected override void OnResize(EventArgs e)
        {
            this.renderer.Resize(this.Width, this.Height);

            base.OnResize(e);
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            if (this.Keyboard[Key.Escape])
            {
                this.Close();
                return;
            }

            this.gamestate.Update(e);
        }

        protected override void OnRender(UpdateEventArgs e)
        {
            this.renderer.Render(this.gamestate);

            this.renderer.FinalizeFrame();
            
            this.SwapBuffers();
        }
    }
}
