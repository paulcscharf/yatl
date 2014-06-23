using amulware.Graphics;
using yatl.Utilities;

namespace yatl.Rendering
{
    sealed class ShaderManager
    {
        public ISurfaceShader UVColor { get; private set; }
        public ISurfaceShader Sprite3D { get; private set; }

        public ISurfaceShader Wall { get; private set; }

        public ISurfaceShader DebugDeferred { get; private set; }

        public ISurfaceShader PointLights { get; private set; }
        public ISurfaceShader AmbientLight { get; private set; }

        public ISurfaceShader PostCopy { get; private set; }

        public ShaderManager()
        {
            this.initialise();
        }

        private void initialise()
        {
            this.UVColor = GraphicsHelper.LoadShaderProgram("data/shaders/uvcolor");
            this.Sprite3D = GraphicsHelper.LoadShaderProgram("data/shaders/simple_sprite");

            this.Wall = GraphicsHelper.LoadShaderProgram("data/shaders/wall");

            this.DebugDeferred = GraphicsHelper.LoadShaderProgram("data/shaders/post.vs", "data/shaders/post/debugDeferred.fs");

            this.PointLights = GraphicsHelper.LoadShaderProgram("data/shaders/deferred/pointlight");
            this.AmbientLight = GraphicsHelper.LoadShaderProgram("data/shaders/deferred/ambient");

            this.PostCopy = GraphicsHelper.LoadShaderProgram("data/shaders/post.vs", "data/shaders/post/copy.fs");
        }
    }
}
