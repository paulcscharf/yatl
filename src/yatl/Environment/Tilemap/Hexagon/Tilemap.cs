using System;
using System.Collections;
using System.Collections.Generic;

namespace yatl.Environment.Tilemap.Hexagon
{
    sealed class Tilemap<TTileInfo> : IEnumerable<Tile<TTileInfo>>
    {
        private readonly int radius;
        private readonly TTileInfo[,] tiles;

        public int Radius { get { return this.radius; } }

        public int Count
        {
            get { return 3 * this.radius * (this.radius + 1) + 1; }
        }

        /* Layout of array:
         * (radius 1)
         * 
         * \_\#\#\
         *  \#\0\#\
         *   \#\#\_\
         * 
         * 0 = 0,0 origin tile
         * # = other tiles
         * _ = empty tiles (not used)
         * 
         */

        public Tilemap(int radius)
        {
            this.radius = radius;
            int arrayDimension = radius * 2 + 1;
            this.tiles = new TTileInfo[arrayDimension, arrayDimension];
        }

        public TTileInfo this[int x, int y]
        {
            get
            {
                this.transformToArray(ref x, ref y);
                return this.tiles[x, y];
            }
            set
            {
                this.transformToArray(ref x, ref y);
                this.tiles[x, y] = value;
            }
        }

        public TTileInfo this[Tile<TTileInfo> tile]
        {
            get { return this[tile.X, tile.Y]; }
            set { this[tile.X, tile.Y] = value; }
        }

        public TTileInfo this[ITile tile]
        {
            get { return this[tile.X, tile.Y]; }
            set { this[tile.X, tile.Y] = value; }
        }

        public bool IsValidTile(int x, int y)
        {
            return x >= -this.radius && x <= this.radius
                && y >= -this.radius && y <= this.radius
                && x + y >= -this.radius && x + y <= this.radius;
        }

        public bool IsValidTile(Tile<TTileInfo> tile)
        {
            return this.IsValidTile(tile.X, tile.Y);
        }
        public bool IsValidTile(ITile tile)
        {
            return this.IsValidTile(tile.X, tile.Y);
        }

        private void transformToArray(ref int x, ref int y)
        {
            x += this.radius;
            y += this.radius;
        }

        public IEnumerable<Tile<TTileInfo>> TilesSpiralOutward
        {
            get
            {
                int x = 0;
                int y = 0;

                yield return new Tile<TTileInfo>(this, 0, 0);

                // for each circle
                for (int r = 0; r < this.radius; r++)
                {
                    y--;

                    // for each edge
                    for (int d = 1; d <= 6; d++)
                    {
                        var step = ((Direction)d).Step();

                        // for each tile
                        for (int t = 0; t <= r; t++)
                        {
                            yield return new Tile<TTileInfo>(this, x, y);

                            x += step.X;
                            y += step.Y;
                        }
                    }

                }
            }
        }

        public IEnumerator<Tile<TTileInfo>> GetEnumerator()
        {
            for (int y = -this.radius; y <= this.radius; y++)
            {
                int xMin = Math.Max(-this.radius, -this.radius - y);
                int xMax = Math.Min(this.radius, this.radius - y);

                for (int x = xMin; x <= xMax; x++)
                {
                    yield return new Tile<TTileInfo>(this, x, y);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
