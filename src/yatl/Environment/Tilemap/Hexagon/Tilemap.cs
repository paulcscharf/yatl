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
            return x > -this.radius && x <= this.radius
                && y > -this.radius && y <= this.radius
                && x + y >= -this.radius && x + y <= this.radius;
        }

        private void transformToArray(ref int x, ref int y)
        {
            x += this.radius;
            y += this.radius;
        }
    }
}
