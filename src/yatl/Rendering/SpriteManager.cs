using amulware.Graphics;
using OpenTK;
using yatl.Rendering.Deferred;
using yatl.Rendering.Hud;
using yatl.Rendering.Walls;

namespace yatl.Rendering
{
    sealed class SpriteManager
    {
        public static SpriteManager Instance { get; private set; }

        public FontGeometry ScreenText { get; private set; }
        public FontGeometry GameText { get; private set; }

        #region Particles

        public Sprite3DGeometry Eyes { get; private set; }

        public Sprite3DGeometry Bloob { get; private set; }
        public Sprite3DGeometry Blink { get; private set; }
        public Sprite3DGeometry Splash { get; private set; }
        public Sprite3DGeometry Fadering { get; private set; }
        public Sprite3DGeometry Sparkly { get; private set; }
        public Sprite3DGeometry Orb { get; private set; }

        #endregion

        public Sprite2DGeometry FilledHexagon { get; private set; }
        public Sprite2DGeometry EmptyHexagon { get; private set; }
        public Sprite2DGeometry Lines { get; private set; }
        public HudGeometry Hud { get; private set; }

        public WallGeometry Wall { get; private set; }

        public DeferredPointLightGeometry PointLight { get; private set; }

        public SpriteManager(SurfaceManager surfaces)
        {
            this.initialise(surfaces);
            SpriteManager.Instance = this;
        }

        private void initialise(SurfaceManager surfaces)
        {
            this.ScreenText = new FontGeometry(surfaces.ScreenFontSurface, surfaces.Font);
            this.GameText = new FontGeometry(surfaces.GameFontSurface, surfaces.Font)
            {SizeCoefficient = new Vector2(1, -1)};

            this.Bloob = (Sprite3DGeometry)surfaces.Particles["bloob"].Geometry;
            this.Blink = (Sprite3DGeometry)surfaces.Particles["blink"].Geometry;
            this.Splash = (Sprite3DGeometry)surfaces.Particles["splash"].Geometry;
            this.Fadering = (Sprite3DGeometry)surfaces.Particles["fadering"].Geometry;
            this.Sparkly = (Sprite3DGeometry)surfaces.Particles["sparkly"].Geometry;
            this.Orb = (Sprite3DGeometry)surfaces.Particles["orb"].Geometry;

            this.Eyes = (Sprite3DGeometry)surfaces.Sprites["eyes"].Geometry;

            this.FilledHexagon = (Sprite2DGeometry)surfaces.Hexagons["filled"].Geometry;
            this.EmptyHexagon = (Sprite2DGeometry)surfaces.Hexagons["empty"].Geometry;
            this.Lines = (Sprite2DGeometry)surfaces.Hexagons["line"].Geometry;
            this.Hud = new HudGeometry(surfaces.Hud);

            this.Wall = new WallGeometry(surfaces.Walls);

            this.PointLight = new DeferredPointLightGeometry(surfaces.PointLights);
        }
    }
}
