using amulware.Graphics;
using OpenTK.Graphics.OpenGL;
using yatl.Environment;

namespace yatl.Rendering
{
    sealed class GameRenderer
    {
        private readonly ShaderManager shaders;
        private readonly SurfaceManager surfaces;
        private readonly SpriteManager sprites;

        public GameRenderer()
        {
            this.shaders = new ShaderManager();
            this.surfaces = new SurfaceManager(this.shaders);
            this.sprites = new SpriteManager(this.surfaces);
        }

        public void Render(GameState state)
        {
            state.Draw(this.sprites);
        }

        public void FinalizeFrame()
        {
            GL.DepthMask(false);
            GL.CullFace(CullFaceMode.FrontAndBack);

            GL.ClearColor(Color.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);


            this.surfaces.ScreenFontSurface.Render();

            
        }
    }
}
