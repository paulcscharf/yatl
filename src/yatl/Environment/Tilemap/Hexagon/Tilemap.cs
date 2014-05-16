using System.Collections.Generic;

namespace yatl.Environment.Tilemap.Hexagon
{
    sealed class Tilemap<TTileInfo>
    {
        private readonly int radius;
        private readonly TTileInfo[,] tiles;

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

        public bool IsValidTile(int x, int y)
        {
            return x >= -this.radius && x <= this.radius
                && y >= -this.radius && y <= this.radius
                && x + y >= -this.radius && x + y <= this.radius;
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
    }
}
