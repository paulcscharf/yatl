using System;
using System.Diagnostics;
using amulware.Graphics;
using OpenTK.Graphics.OpenGL;
using yatl.Environment;
using yatl.Utilities;

namespace yatl.Rendering
{
    sealed class GameRenderer
    {
        private int screenWidth;
        private int screenHeight;
        private float aspectRatio;
        private int scissorX;
        private int scissorY;
        private int scissorW;
        private int scissorH;

        private readonly ShaderManager shaders;
        private readonly SurfaceManager surfaces;
        private readonly SpriteManager sprites;

        private Stopwatch shaderReloadTimer;

        public GameRenderer()
        {
            this.shaders = new ShaderManager();
            this.surfaces = new SurfaceManager(this.shaders);
            this.sprites = new SpriteManager(this.surfaces);

            this.shaderReloadTimer = Stopwatch.StartNew();
        }

        public void Render(GameState state)
        {
            state.Draw(this.sprites);

            this.surfaces.SetGameCamera(state.Camera);
        }

        public void FinalizeFrame()
        {
            #region Reload Shaders
#if DEBUG
            if (this.shaderReloadTimer.Elapsed > TimeSpan.FromSeconds(0.2))
            {
                GraphicsHelper.CheckAndUpdateChangedShaders();
                this.shaderReloadTimer.Restart();
            }
#endif
            #endregion

            #region Global Settings

            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);

            #endregion

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);


            GL.Viewport(0, 0, this.screenWidth, this.screenHeight);

            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            GL.Viewport(this.scissorX, this.scissorY, this.scissorW, this.scissorH);
            GL.Scissor(this.scissorX, this.scissorY, this.scissorW, this.scissorH);

            GL.Enable(EnableCap.ScissorTest);
            GL.ClearColor(Color.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(EnableCap.ScissorTest);

            #region Draw Game

            GL.Enable(EnableCap.DepthTest);

            this.surfaces.Walls.Render();
            
            this.surfaces.Particles.Surface.Render();

            this.surfaces.Hexagons.Surface.Render();


            this.surfaces.GameFontSurface.Render();

            GL.Disable(EnableCap.DepthTest);

            #endregion

            #region Draw Interface

            this.surfaces.ScreenFontSurface.Render();

            #endregion
        }

        public void Resize(int width, int height)
        {
            this.screenWidth = width;
            this.screenHeight = height;

            this.aspectRatio = (float)width / height;

            if (this.aspectRatio >= 16.0 / 9.0)
            {
                int w = (int)(height * 16.0 / 9.0);
                int x = (width - w) / 2;

                this.scissorX = x;
                this.scissorY = 0;
                this.scissorW = w;
                this.scissorH = this.screenHeight;
            }
            else
            {
                int h = (int)(width * 9.0 / 16.0);
                int y = (height - h) / 2;

                this.scissorX = 0;
                this.scissorY = y;
                this.scissorW = this.screenWidth;
                this.scissorH = h;
            }
        }
    }
}
