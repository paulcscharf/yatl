using amulware.Graphics;
using OpenTK;
using yatl.Rendering.Hud;
using yatl.Rendering.Walls;

namespace yatl.Rendering
{
    sealed class SpriteManager
    {
        public FontGeometry ScreenText { get; private set; }
        public FontGeometry GameText { get; private set; }

        #region Particles

        public Sprite2DGeometry Bloob { get; private set; }
        public Sprite2DGeometry Blink { get; private set; }
        public Sprite2DGeometry Splash { get; private set; }
        public Sprite2DGeometry Fadering { get; private set; }
        public Sprite2DGeometry Sparkly { get; private set; }
        public Sprite2DGeometry Orb { get; private set; }

        #endregion

        public Sprite2DGeometry FilledHexagon { get; private set; }
        public Sprite2DGeometry EmptyHexagon { get; private set; }
        public Sprite2DGeometry Lines { get; private set; }
        public HudGeometry Hud { get; private set; }

        public WallGeometry Wall { get; private set; }

        public SpriteManager(SurfaceManager surfaces)
        {
            this.initialise(surfaces);
        }

        private void initialise(SurfaceManager surfaces)
        {
            this.ScreenText = new FontGeometry(surfaces.ScreenFontSurface, surfaces.Font);
            this.GameText = new FontGeometry(surfaces.GameFontSurface, surfaces.Font)
            {SizeCoefficient = new Vector2(1, -1)};

            this.Bloob = (Sprite2DGeometry) surfaces.Particles["bloob"].Geometry;
            this.Blink = (Sprite2DGeometry) surfaces.Particles["blink"].Geometry;
            this.Splash = (Sprite2DGeometry) surfaces.Particles["splash"].Geometry;
            this.Fadering = (Sprite2DGeometry) surfaces.Particles["fadering"].Geometry;
            this.Sparkly = (Sprite2DGeometry) surfaces.Particles["sparkly"].Geometry;
            this.Orb = (Sprite2DGeometry) surfaces.Particles["orb"].Geometry;

            this.FilledHexagon = (Sprite2DGeometry)surfaces.Hexagons["filled"].Geometry;
            this.EmptyHexagon = (Sprite2DGeometry)surfaces.Hexagons["empty"].Geometry;
            this.Lines = (Sprite2DGeometry)surfaces.Hexagons["line"].Geometry;
            this.Hud = new HudGeometry(surfaces.Hud);

            this.Wall = new WallGeometry(surfaces.Walls);
        }
    }
}
