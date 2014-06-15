using System;
using System.Linq;
using amulware.Graphics;
using OpenTK;
using yatl.Environment.Level.Generation;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Rendering;
using yatl.Rendering.Deferred;
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

        public Tilemap<TileInfo> Tilemap { get { return this.tilemap; } }

        private IndexedSurface<DeferredAmbientLightVertex> ambientLightSurface;

        public Level(GameState game, LevelGenerator generator)
        {
            this.game = game;

            this.tilemap = generator.Generate();

            foreach (var tile in this.tilemap)
                tile.Info.InitGeometry(this.GetPosition(tile));

            this.createAmbient();
        }

        private void createAmbient()
        {
            var ambientTiles = new Tilemap<ushort>(this.tilemap.Radius + 1);

            var vertices = new DeferredAmbientLightVertex[ambientTiles.Count];

            ushort index = 0;

            foreach (var tile in ambientTiles)
            {
                float z = 0;
                var argb = Color.White;
                if (this.tilemap.IsValidTile(tile))
                {
                    z = Settings.Game.Level.WallHeight;
                    argb = this.tilemap[tile].AmbientColor;
                }

                vertices[index] = new DeferredAmbientLightVertex(this.GetPosition(tile).WithZ(z), argb);
                ambientTiles[tile] = index;

                index++;
            }

            var halfDirs = new[]
            {
                Direction.Left,
                Direction.DownLeft,
                //Direction.DownRight
            };

            var indices = new ushort[(ambientTiles.Count - 1) * 6];

            var i = 0;

            // ReSharper disable AccessToForEachVariableInClosure
            foreach (var tile in ambientTiles)
                foreach (var ns in halfDirs
                    .Select(d => new { N1 = tile.Neighbour(d), N2 = tile.Neighbour(d + 1) })
                    .Where(ns => ambientTiles.IsValidTile(ns.N1) && ambientTiles.IsValidTile(ns.N2)))
                {
                    indices[i++] = tile.Info;
                    indices[i++] = ns.N2.Info;
                    indices[i++] = ns.N1.Info;
                }
            // ReSharper restore AccessToForEachVariableInClosure

            this.ambientLightSurface = SurfaceManager.Instance.MakeAmbientLightSurface();
            this.ambientLightSurface.AddVertices(vertices);
            this.ambientLightSurface.AddIndices(indices);
        }

        public TiledRayHitResult ShootRay(Ray ray)
        {
            return this.ShootRay(ray, this.GetTile(ray.Start));
        }

        public TiledRayHitResult ShootRay(Ray ray, Tile<TileInfo> startTile)
        {
            var tile = startTile;
            var endTile = this.GetTile(ray.Start + ray.Direction);
            while (true)
            {
                var offset = this.GetPosition(tile);
                var rayLocal = new Ray(ray.Start - offset, ray.Direction);
                var result = tile.Info.ShootRay(rayLocal);
                if (result.RayFactor < 1)
                    return result.OnTile(tile, offset);

                if (tile == endTile)
                    return result.OnTile(tile, offset);

                tile = tile.Neighbour(
                    tile.Info.GetOutDirection(rayLocal));
            }
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

        public Vector2 GetPosition(ITile tile)
        {
            return Settings.Game.Level.TileToPosition * new Vector2(tile.X, tile.Y);
        }

        public void DisposeGeometry()
        {
            foreach (var tile in this.tilemap)
                tile.Info.DisposeGeometry();
            SurfaceManager.Instance.DisposeOfAmbientLightSurface(this.ambientLightSurface);
        }

        public void Draw(SpriteManager sprites)
        {
            foreach (var tile in this.tilemap)
                tile.Info.Draw();

            SurfaceManager.Instance.QueueLight(this.ambientLightSurface);

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
                lines.Color = new Color(0, 50, 0, 0);

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

                    font.DrawString(position3D, tile.Radius.ToString(), 0.5f, 1);
                    font.DrawString(position3D, tile.X + "," + tile.Y, 0.5f, 0);


                    i++;
                }
            }
        }
    }
}
