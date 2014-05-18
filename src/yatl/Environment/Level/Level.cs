using System;
using amulware.Graphics;
using OpenTK;
using yatl.Environment.Level.Generation;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Rendering;
using yatl.Utilities;
using Direction = yatl.Environment.Tilemap.Hexagon.Direction;

namespace yatl.Environment.Level
{
    sealed class Level
    {
        private static readonly Direction[] activeDirections =
        {
            Direction.Left,
            Direction.DownLeft,
            Direction.DownRight,
        };

        private readonly GameState game;
        private readonly Tilemap<TileInfo> tilemap;

        public Level(GameState game, LevelGenerator generator)
        {
            this.game = game;

            this.tilemap = generator.Generate();
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

                var hexFull = sprites.FilledHexagon;

                var lines = sprites.Lines;
                lines.LineWidth = 0.5f;
                lines.Color = Color.Green;

                int i = 0;
                foreach (var tile in this.tilemap)
                {
                    var position = this.GetPosition(tile);

                    if (tile.Info.OpenSides.Any())
                    {
                        // draw center piece
                        hexFull.Color = lines.Color;
                        hexFull.DrawSprite(position, 30f.Degrees().Radians, lines.LineWidth * 1.87f);

                        // show navigation graph
                        foreach (var direction in Level.activeDirections)
                        {
                            if (!tile.Info.OpenSides.Includes(direction))
                                continue;

                            var next = tile.Neighbour(direction);

                            var positionNext = this.GetPosition(next);

                            lines.DrawLine(position, positionNext);
                        } 
                    }


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
