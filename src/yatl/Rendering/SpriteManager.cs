using amulware.Graphics;

namespace yatl.Rendering
{
    sealed class SpriteManager
    {
        public FontGeometry ScreenText { get; private set; }

        #region Particles

        public Sprite2DGeometry Bloob { get; private set; }
        public Sprite2DGeometry Blink { get; private set; }
        public Sprite2DGeometry Splash { get; private set; }
        public Sprite2DGeometry Fadering { get; private set; }
        public Sprite2DGeometry Sparkly { get; private set; }
        public Sprite2DGeometry Orb { get; private set; }

        #endregion

        public SpriteManager(SurfaceManager surfaces)
        {
            this.initialise(surfaces);
        }

        private void initialise(SurfaceManager surfaces)
        {
            this.ScreenText = new FontGeometry(surfaces.ScreenFontSurface, surfaces.Font);

            this.Bloob = (Sprite2DGeometry) surfaces.Particles["bloob"].Geometry;
            this.Blink = (Sprite2DGeometry) surfaces.Particles["blink"].Geometry;
            this.Splash = (Sprite2DGeometry) surfaces.Particles["splash"].Geometry;
            this.Fadering = (Sprite2DGeometry) surfaces.Particles["fadering"].Geometry;
            this.Sparkly = (Sprite2DGeometry) surfaces.Particles["sparkly"].Geometry;
            this.Orb = (Sprite2DGeometry) surfaces.Particles["orb"].Geometry;
        }
    }
}
