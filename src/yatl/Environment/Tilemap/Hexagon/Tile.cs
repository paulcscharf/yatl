using System;
using System.Collections.Generic;
using System.Linq;

namespace yatl.Environment.Tilemap.Hexagon
{
    internal struct Tile<TTileInfo>
    {
        private readonly Tilemap<TTileInfo> tilemap;
        public readonly int X;
        public readonly int Y;

        public Tile(Tilemap<TTileInfo> tilemap, int x, int y)
        {
            if (tilemap == null)
                throw new ArgumentNullException("tilemap");

            this.tilemap = tilemap;
            this.X = x;
            this.Y = y;
        }

        public TTileInfo Info
        {
            get { return this.tilemap[this.X, this.Y]; }
        }

        public bool IsValid
        {
            get { return this.tilemap.IsValidTile(this.X, this.Y); }
        }

        public Tile<TTileInfo> Neighbour(Direction direction)
        {
            return this.Neighbour(direction.Step());
        }

        public Tile<TTileInfo> Neighbour(Step step)
        {
            return new Tile<TTileInfo>(
                this.tilemap,
                this.X + step.X,
                this.Y + step.Y
                );
        }

        public IEnumerable<Tile<TTileInfo>> Neighbours
        {
            get { return this.PossibleNeighbours().Where(t => t.IsValid); }
        }
    }
}
