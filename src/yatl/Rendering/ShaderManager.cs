using amulware.Graphics;
using yatl.Utilities;

namespace yatl.Rendering
{
    sealed class ShaderManager
    {
        public ISurfaceShader UVColor { get; private set; }
        public ISurfaceShader Wall { get; private set; }

        public ISurfaceShader DebugDeferred { get; private set; }

        public ISurfaceShader PointLights { get; private set; }

        public ShaderManager()
        {
            this.initialise();
        }

        private void initialise()
        {
            this.UVColor = GraphicsHelper.LoadShaderProgram("data/shaders/uvcolor");
            this.Wall = GraphicsHelper.LoadShaderProgram("data/shaders/wall");

            this.DebugDeferred = GraphicsHelper.LoadShaderProgram("data/shaders/post.vs", "data/shaders/post/debugDeferred.fs");

            this.PointLights = GraphicsHelper.LoadShaderProgram("data/shaders/deferred/pointlight");
        }
    }
}
