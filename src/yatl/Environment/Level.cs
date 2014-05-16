using System.Linq;
using amulware.Graphics;
using OpenTK;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Rendering;

namespace yatl.Environment
{
    sealed class Level
    {
        private readonly Tilemap<object> tilemap;

        public Level()
        {
            this.tilemap = new Tilemap<object>(Settings.Game.Level.Radius);
        }

        public void Draw(SpriteManager sprites)
        {
            var hex = sprites.EmptyHexagon;
            var font = sprites.GameText;
            font.Height = 0.5f;

            int i = 0;
            foreach (var tile in this.tilemap.TilesSpiralOutward)
            {
                var argb = Color.Black;

                var position = tile.X * Settings.Game.Level.HexagonGridUnitX
                               + tile.Y * Settings.Game.Level.HexagonGridUnitY;

                hex.Color = argb;
                hex.DrawSprite(position, 0, Settings.Game.Level.HexagonDiameter);

                font.Color = argb;
                font.DrawString(position, i.ToString(), 0.5f, 1);
                font.DrawString(position, tile.X + "," + tile.Y, 0.5f, 0);

                i++;
            }
        }
    }
}
