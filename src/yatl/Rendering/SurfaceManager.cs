
using System;
using amulware.Graphics;
using OpenTK;
using yatl.Utilities;

namespace yatl.Rendering
{
    sealed class SurfaceManager
    {
        private Matrix4Uniform gameProjection;
        private Matrix4Uniform gameModelview;
        private Matrix4Uniform screenModelview;

        private TextureUniform fontTextureUniform;
        public Font Font { get; private set; }
        public IndexedSurface<UVColorVertexData> ScreenFontSurface { get; private set; }

        public SurfaceManager(ShaderManager shaders)
        {
            this.initialise(shaders);
        }

        private void initialise(ShaderManager shaders)
        {
            this.initMatrices();
            this.initFonts(shaders);
        }

        private void initMatrices()
        {
            this.gameProjection = new Matrix4Uniform("projectionMatrix");
            this.gameModelview = new Matrix4Uniform("modelviewMatrix");

            this.screenModelview = new Matrix4Uniform("modelviewMatrix");

            this.makeGameProjectionMatrix();
            this.makeScreenModelviewMatrix();
        }

        private void initFonts(ShaderManager shaders)
        {
            this.Font = Font.FromJsonFile("data/gfx/fonts/calibri.json");
            this.fontTextureUniform = new TextureUniform("diffuseTexture",
                new Texture("data/gfx/fonts/calibri.png", true));

            this.ScreenFontSurface = new IndexedSurface<UVColorVertexData>();
            this.ScreenFontSurface.AddSettings(
                this.screenModelview,
                this.gameProjection,
                this.fontTextureUniform,
                SurfaceBlendSetting.PremultipliedAlpha
                );
            shaders.UVColor.UseOnSurface(this.ScreenFontSurface);
        }

        private void makeGameProjectionMatrix()
        {
            const float zNear = 0.1f;
            const float zFar = 256f;
            const float fovy = (float)Math.PI / 4;

            const float ratio = 16f / 9f;

            float yMax = zNear * (float)Math.Tan(0.5f * fovy);
            float yMin = -yMax;
            float xMin = yMin * ratio;
            float xMax = yMax * ratio;

            this.gameProjection.Matrix = Matrix4.CreatePerspectiveOffCenter(xMin, xMax, yMin, yMax, zNear, zFar);

        }

        private void makeScreenModelviewMatrix()
        {
            Vector3 topCenter = new Vector3(0, 9, 0);
            this.screenModelview.Matrix = Matrix4.LookAt(
                new Vector3(0, 0, -22) + topCenter, new Vector3(0, 0, 0) + topCenter, -Vector3.UnitY);
        }
    }

    sealed class ShaderManager
    {
        public ISurfaceShader UVColor { get; private set; }

        public ShaderManager()
        {
            this.initialise();
        }

        private void initialise()
        {
            this.UVColor = GraphicsHelper.LoadShaderProgram("data/shaders/uvcolor");
        }
    }
}
