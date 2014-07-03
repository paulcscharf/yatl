using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using amulware.Graphics;
using Cireon.Audio;
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
        private MusicManager musicManager;
        private Thread musicThread;
        private WindowState previousWindowState = WindowState.Fullscreen;

        public YATLWindow(int glMajor, int glMinor)
            : base(Settings.General.DefaultWindowWidth,
                Settings.General.DefaultWindowHeight,
                GraphicsMode.Default, "You are the light",
                GameWindowFlags.Default, DisplayDevice.Default, glMajor, glMinor,
                GraphicsContextFlags.Default)
        {
            Console.WriteLine("GL Context Version: {0}", GL.GetString(StringName.Version));
            Console.WriteLine("GL Shader Version: {0}", GL.GetString(StringName.ShadingLanguageVersion));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InputManager.Initialize(this.Mouse);

            this.renderer = new GameRenderer();

            this.musicManager = new MusicManager();

            this.musicThread = new Thread(this.updateMusic);

            this.gamestate = new GameState(true);

            this.musicThread.Start();
        }

        private void updateMusic()
        {
            var args = new UpdateEventArgs(0);
            var timer = Stopwatch.StartNew();
            while (true)
            {
                args = new UpdateEventArgs(args, timer.Elapsed.TotalSeconds);
                this.musicManager.Update(args);
                
                Thread.Sleep(TimeSpan.FromMilliseconds(2));
            }


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

            if(this.Focused)
                InputManager.Update();

            if(InputManager.IsKeyHit(Key.F12))
                this.MakeScreenshot();

            if (InputManager.IsKeyHit(Key.F11))
            {
                var tempState = this.previousWindowState;
                this.previousWindowState = this.WindowState;
                this.WindowState = tempState;

            }
            

            if (this.gamestate.WaitingForReset || InputManager.IsKeyHit(Key.F5))
                this.restartGame();

            this.gamestate.Update(e);

            this.musicManager.Parameters = this.gamestate.MusicParameters;

            //this.musicManager.Update(e);
        }

        private void restartGame()
        {
            if(this.gamestate != null)
                this.gamestate.Dispose();
            this.gamestate = new GameState();
        }

        protected override void OnRender(UpdateEventArgs e)
        {
            if (this.IsExiting)
                return;

            this.renderer.Render(this.gamestate);

            this.renderer.FinalizeFrame();
            
            this.SwapBuffers();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.musicThread.Abort();
            AudioManager.Instance.Dispose();

            base.OnClosing(e);
        }
    }
}
