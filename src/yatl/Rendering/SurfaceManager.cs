
using System;
using System.Collections.Generic;
using amulware.Graphics;
using OpenTK;
using yatl.Environment;
using yatl.Rendering.Deferred;
using yatl.Rendering.Walls;

namespace yatl.Rendering
{
    sealed class SurfaceManager
    {
        public static SurfaceManager Instance { get; private set; }

        private readonly ShaderManager shaders;
        private readonly DeferredBuffer deferredBuffer;

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

        public SpriteSet<SimpleSpriteVertexData> Sprites { get; private set; }

        public SpriteSet<SimpleSpriteVertexData> Particles { get; private set; }

        public SpriteSet<UVColorVertexData> Hexagons { get; private set; }

        public SpriteSet<UVColorVertexData> Hud { get; private set; }

        public SpriteSet<UVColorVertexData> Tutorial { get; private set; } 

        #endregion

        #region Level Geometry

        public IndexedSurface<WallVertex> Walls { get; private set; }

        #endregion

        #region Deferred

        public IndexedSurface<DeferredPointLightVertex> PointLights { get; set; }
        
        #endregion

        public PostProcessSurface Overlay { get; private set; }

        private FloatUniform overlayFadePercentage;
        public float OverlayFadePercentage { set { this.overlayFadePercentage.Float = value; } }
        private Vector4Uniform overlayColor;
        public Vector4 OverlayColor { set { this.overlayColor.Vector = value; } }

        public SurfaceManager(ShaderManager shaders, DeferredBuffer deferredBuffer)
        {
            this.shaders = shaders;
            this.deferredBuffer = deferredBuffer;
            this.initialise(shaders, deferredBuffer);
            SurfaceManager.Instance = this;
        }

        private void initialise(ShaderManager shaders, DeferredBuffer deferredBuffer)
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

            this.initSprites(shaders);

            this.initHexagons(shaders);
            this.initHud(shaders);
            this.initTutorial(shaders);

            this.initWalls(shaders);

            this.initDeferred(shaders, deferredBuffer);

            this.initOverlay(shaders);
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
            this.Particles = this.load3DGameSpriteSet(shaders, "particles");
        }

        private void initSprites(ShaderManager shaders)
        {
            this.Sprites = this.load3DGameSpriteSet(shaders, "sprites");
        }

        private void initHexagons(ShaderManager shaders)
        {
            this.Hexagons = this.loadGameSpriteSet(shaders, "hexagons");
        }

        private void initHud(ShaderManager shaders)
        {
            this.Hud = SpriteSet<UVColorVertexData>.FromJsonFile(
                "data/gfx/sprites/hud.json", s => new Sprite2DGeometry(s),
                shaders.UVColor, new SurfaceSetting[] { this.screenModelview, this.gameProjection, SurfaceBlendSetting.PremultipliedAlpha },
                SurfaceManager.premultiplyTexture, true);
        }
        private void initTutorial(ShaderManager shaders)
        {
            this.Tutorial = SpriteSet<UVColorVertexData>.FromJsonFile(
                "data/gfx/sprites/tutorial.json", s => new Sprite2DGeometry(s),
                shaders.UVColor, new SurfaceSetting[] { this.screenModelview, this.gameProjection, SurfaceBlendSetting.PremultipliedAlpha },
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

        private void initDeferred(ShaderManager shaders, DeferredBuffer deferredBuffer)
        {
            this.PointLights = new IndexedSurface<DeferredPointLightVertex>();
            this.PointLights.AddSettings(
                this.gameModelview,
                this.gameProjection,
                SurfaceBlendSetting.PremultipliedAlpha,
                deferredBuffer
                );
            shaders.PointLights.UseOnSurface(this.PointLights);
        }

        private void initOverlay(ShaderManager shaders)
        {
            this.Overlay = new PostProcessSurface();
            this.Overlay.AddSettings(
                SurfaceBlendSetting.PremultipliedAlpha,
                this.overlayFadePercentage = new FloatUniform("fadePercentage"),
                this.overlayColor = new Vector4Uniform("color")
                );
            shaders.Overlay.UseOnSurface(this.Overlay);
        }



        private SpriteSet<UVColorVertexData> loadGameSpriteSet(ShaderManager shaders, string filename)
        {
            return SpriteSet<UVColorVertexData>.FromJsonFile(
                "data/gfx/sprites/" + filename + ".json", s => new Sprite2DGeometry(s),
                shaders.UVColor, this.gameSpriteSettings, SurfaceManager.premultiplyTexture, true);
        }

        private SpriteSet<SimpleSpriteVertexData> load3DGameSpriteSet(ShaderManager shaders, string filename)
        {
            return SpriteSet<SimpleSpriteVertexData>.FromJsonFile(
                "data/gfx/sprites/" + filename + ".json", s => new Sprite3DGeometry(s),
                shaders.Sprite3D, this.gameSpriteSettings, SurfaceManager.premultiplyTexture, true);
        }

        private void makeGameProjectionMatrix()
        {
            const float zNear = 1f;
            const float zFar = 512f;
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

        public IndexedSurface<WallVertex> MakeLevelGeometrySurface()
        {
            var surface = new IndexedSurface<WallVertex>
            {
                ClearOnRender = false,
                IsStatic = true,
            };
            surface.AddSettings(
                this.gameModelview,
                this.gameProjection
                );
            this.shaders.Wall.UseOnSurface(surface);
            return surface;
        }

        public void DisposeOfLevelGeometrySurface(IndexedSurface<WallVertex> surface)
        {
            var refresher = this.shaders.Wall as ShaderProgramRefresher;
            if (refresher == null)
                return;
            refresher.RemoveFromSurface(surface);
        }

        public IndexedSurface<DeferredAmbientLightVertex> MakeAmbientLightSurface()
        {
            var surface = new IndexedSurface<DeferredAmbientLightVertex>
            {
                ClearOnRender = false,
                IsStatic = true,
            };
            surface.AddSettings(
                this.gameModelview,
                this.gameProjection,
                SurfaceBlendSetting.Add,
                this.deferredBuffer
                );
            this.shaders.AmbientLight.UseOnSurface(surface);
            return surface;
        }

        public void DisposeOfAmbientLightSurface(IndexedSurface<DeferredAmbientLightVertex> surface)
        {
            var refresher = this.shaders.AmbientLight as ShaderProgramRefresher;
            if (refresher == null)
                return;
            refresher.RemoveFromSurface(surface);
        }


        private readonly List<Surface> levelGeoQueue = new List<Surface>();

        public IEnumerable<Surface> LevelGeometryQueue { get { return this.levelGeoQueue; } } 

        public void QueueLevelGeometry(IndexedSurface<WallVertex> surface)
        {
            this.levelGeoQueue.Add(surface);
        }

        private readonly List<Surface> lightQueue = new List<Surface>();

        public IEnumerable<Surface> LightQueue { get { return this.lightQueue; } }

        public void QueueLight(IndexedSurface<DeferredAmbientLightVertex> surface)
        {
            this.lightQueue.Add(surface);
        }

        public void ClearQueues()
        {
            this.levelGeoQueue.Clear();
            this.lightQueue.Clear();
        }
    }
}
