using amulware.Graphics;

namespace yatl.Rendering
{
    sealed class SpriteManager
    {
        public FontGeometry ScreenText { get; private set; }

        public SpriteManager(SurfaceManager surfaces)
        {
            this.initialise(surfaces);
        }

        private void initialise(SurfaceManager surfaces)
        {
            this.ScreenText = new FontGeometry(surfaces.ScreenFontSurface, surfaces.Font);
        }
    }
}
