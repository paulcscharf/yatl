using System;
using System.Collections.Generic;
using System.Linq;

namespace yatl.Environment.Tilemap.Hexagon
{
    internal struct Tile<TTileInfo>
    {
        private readonly Tilemap<TTileInfo> tilemap;
        private readonly int x;
        private readonly int y;

        public Tile(Tilemap<TTileInfo> tilemap, int x, int y)
        {
            if (tilemap == null)
                throw new ArgumentNullException("tilemap");

            this.tilemap = tilemap;
            this.x = x;
            this.y = y;
        }

        public TTileInfo Info
        {
            get { return this.tilemap[this.x, this.y]; }
        }

        public bool IsValid
        {
            get { return this.tilemap.IsValidTile(this.x, this.y); }
        }

        public Tile<TTileInfo> Neighbour(Direction direction)
        {
            return this.Neighbour(direction.Step());
        }

        public Tile<TTileInfo> Neighbour(Step step)
        {
            return new Tile<TTileInfo>(
                this.tilemap,
                this.x + step.X,
                this.y + step.Y
                );
        }

        public IEnumerable<Tile<TTileInfo>> Neighbours
        {
            get { return this.PossibleNeighbours().Where(t => t.IsValid); }
        }
    }
}
