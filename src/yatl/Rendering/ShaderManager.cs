using amulware.Graphics;
using yatl.Utilities;

namespace yatl.Rendering
{
    sealed class ShaderManager
    {
        public ISurfaceShader UVColor { get; private set; }
        public ISurfaceShader Wall { get; private set; }

        public ShaderManager()
        {
            this.initialise();
        }

        private void initialise()
        {
            this.UVColor = GraphicsHelper.LoadShaderProgram("data/shaders/uvcolor");
            this.Wall = GraphicsHelper.LoadShaderProgram("data/shaders/wall");
        }
    }
}
