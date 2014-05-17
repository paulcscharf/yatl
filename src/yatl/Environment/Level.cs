using System;
using amulware.Graphics;
using OpenTK;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Rendering;

namespace yatl.Environment
{
    sealed class Level
    {
        private readonly GameState game;
        private readonly Tilemap<TileInfo> tilemap;

        public Level(GameState game)
        {
            this.game = game;
            this.tilemap = new Tilemap<TileInfo>(Settings.Game.Level.Radius);
        }

        public Tile<TileInfo> GetTile(Vector2 position)
        {
            float yf = position.Y * (1 / Settings.Game.Level.HexagonHeight) + 1 / 1.5f;

            int y = (int)Math.Floor(yf);

            float xf = position.X * (1 / Settings.Game.Level.HexagonWidth) - y * 0.5f + 0.5f;

            int x = (int)Math.Floor(xf);

            float xRemainder = (xf - x) - 0.5f;
            float yRemainder = (yf - y) * 1.5f;

            if (xRemainder > yRemainder)
            {
                x++;
                y--;
            }
            else if (-xRemainder > yRemainder)
                y--;

            return new Tile<TileInfo>(this.tilemap, x, y);
        }

        public Vector2 GetPosition(Tile<TileInfo> tile)
        {
            return Settings.Game.Level.TileToPosition * new Vector2(tile.X, tile.Y);
        }

        public void Draw(SpriteManager sprites)
        {
            if(this.game.DrawDebug)
            {
                var hex = sprites.EmptyHexagon;
                var font = sprites.GameText;
                font.Height = 2;

                hex.Color = Color.GrayScale(20, 0);
                font.Color = Color.White;

                int i = 0;
                foreach (var tile in this.tilemap.TilesSpiralOutward)
                {
                    var position = this.GetPosition(tile);

                    var position3D = new Vector3(position.X, position.Y, Settings.Game.Level.OverlayHeight);

                    hex.DrawSprite(position3D, 0, Settings.Game.Level.HexagonDiameter);

                    font.DrawString(position3D, i.ToString(), 0.5f, 1);
                    font.DrawString(position3D, tile.X + "," + tile.Y, 0.5f, 0);

                    i++;
                }
            }
        }
    }
}
