
using System;
using amulware.Graphics;
using OpenTK;
using yatl.Environment;
using yatl.Rendering.Walls;

namespace yatl.Rendering
{
    sealed class SurfaceManager
    {
        private static Func<string, Texture> premultiplyTexture = file => new Texture(file, true);

        private SurfaceSetting[] gameSpriteSettings;

        #region Matrices

        private Matrix4Uniform gameProjection;
        private Matrix4Uniform gameModelview;
        private Matrix4Uniform screenModelview;

        #endregion

        #region Fonts

        private TextureUniform fontTextureUniform;
        public Font Font { get; private set; }
        public IndexedSurface<UVColorVertexData> ScreenFontSurface { get; private set; }
        public IndexedSurface<UVColorVertexData> GameFontSurface { get; private set; }

        #endregion

        #region Sprites

        public SpriteSet<UVColorVertexData> Particles { get; private set; }

        public SpriteSet<UVColorVertexData> Hexagons { get; private set; }

        public SpriteSet<UVColorVertexData> Hud { get; private set; } 

        #endregion

        #region Level Geometry

        public IndexedSurface<WallVertex> Walls { get; private set; }

        #endregion

        public SurfaceManager(ShaderManager shaders)
        {
            this.initialise(shaders);
        }

        private void initialise(ShaderManager shaders)
        {
            this.initMatrices();

            this.gameSpriteSettings = new SurfaceSetting[]
            {
                this.gameModelview,
                this.gameProjection,
                SurfaceBlendSetting.PremultipliedAlpha
            };

            this.initFonts(shaders);
            this.initParticles(shaders);
            this.initHexagons(shaders);
            this.initHud(shaders);

            this.initWalls(shaders);
        }

        private void initMatrices()
        {
            this.gameProjection = new Matrix4Uniform("projectionMatrix");
            this.gameModelview = new Matrix4Uniform("modelviewMatrix");

            this.screenModelview = new Matrix4Uniform("modelviewMatrix");

            this.makeGameProjectionMatrix();
            this.makeGameModelviewMatrix();
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

            this.GameFontSurface = new IndexedSurface<UVColorVertexData>();
            this.GameFontSurface.AddSettings(
                this.gameModelview,
                this.gameProjection,
                this.fontTextureUniform,
                SurfaceBlendSetting.PremultipliedAlpha
                );
            shaders.UVColor.UseOnSurface(this.GameFontSurface);
        }

        private void initParticles(ShaderManager shaders)
        {
            this.Particles = this.loadGameSpriteSet(shaders, "particles");
        }

        private void initHexagons(ShaderManager shaders)
        {
            this.Hexagons = this.loadGameSpriteSet(shaders, "hexagons");
        }

        private void initHud(ShaderManager shaders)
        {
            this.Hud = SpriteSet<UVColorVertexData>.FromJsonFile(
                "data/gfx/sprites/hud.json", s => new Sprite2DGeometry(s),
                shaders.UVColor, new SurfaceSetting[]
                {this.screenModelview, this.gameProjection, SurfaceBlendSetting.PremultipliedAlpha},
                SurfaceManager.premultiplyTexture, true);
        }

        private void initWalls(ShaderManager shaders)
        {
            this.Walls = new IndexedSurface<WallVertex>();
            this.Walls.AddSettings(
                this.gameModelview,
                this.gameProjection
                );
            shaders.Wall.UseOnSurface(this.Walls);
        }

        private SpriteSet<UVColorVertexData> loadGameSpriteSet(ShaderManager shaders, string filename)
        {
            return SpriteSet<UVColorVertexData>.FromJsonFile(
                "data/gfx/sprites/" + filename + ".json", s => new Sprite2DGeometry(s),
                shaders.UVColor, this.gameSpriteSettings, SurfaceManager.premultiplyTexture, true);
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

        private void makeGameModelviewMatrix()
        {
            this.gameModelview.Matrix = Matrix4.LookAt(new Vector3(0, 0, 25), new Vector3(0, 0, 0), Vector3.UnitY);
        }

        private void makeScreenModelviewMatrix()
        {
            Vector3 topCenter = new Vector3(0, 9, 0);
            this.screenModelview.Matrix = Matrix4.LookAt(
                new Vector3(0, 0, -22) + topCenter, new Vector3(0, 0, 0) + topCenter, -Vector3.UnitY);
        }

        public void SetGameCamera(Camera camera)
        {
            var projectedOnFloor = camera.Focus.Xy - camera.Position.Xy;

            var upVector = Vector3.Cross(
                new Vector3(projectedOnFloor),
                new Vector3(projectedOnFloor.PerpendicularLeft)
                );

            this.gameModelview.Matrix = Matrix4.LookAt(camera.Position, camera.Focus, upVector);
        }
    }
}
