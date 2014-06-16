using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using OpenTK;

namespace yatl.Environment.Tilemap.Hexagon
{
    interface ITile
    {
        int X { get; }
        int Y { get; }
    }

    struct Tile<TTileInfo> : ITile, IEquatable<Tile<TTileInfo>>
    {
        private readonly Tilemap<TTileInfo> tilemap;

        private readonly int x;
        private readonly int y;

        public int Radius
        {
            get
            {
                return this.x * this.y >= 0
                    ? Math.Abs(this.x + this.y)
                    : Math.Max(Math.Abs(this.x), Math.Abs(this.y));
            }
        }

        public int X { get { return this.x; } }
        public int Y { get { return this.y; } }

        public Tile(Tilemap<TTileInfo> tilemap, int x, int y)
        {
            if (tilemap == null)
                throw new ArgumentNullException("tilemap");

            this.tilemap = tilemap;
            this.x = x;
            this.y = y;
        }

        public Vector2 Xy { get { return new Vector2(this.x, this.y); } }

        public TTileInfo Info
        {
            get { return this.tilemap[this]; }
        }

        public bool IsValid
        {
            get { return this.tilemap != null && this.tilemap.IsValidTile(this); }
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

        public bool Equals(Tile<TTileInfo> other)
        {
            return this.x == other.x && this.y == other.y && this.tilemap == other.tilemap;
        }

        public static bool operator ==(Tile<TTileInfo> t1, Tile<TTileInfo> t2)
        {
            return t1.Equals(t2);
        }

        public static bool operator !=(Tile<TTileInfo> t1, Tile<TTileInfo> t2)
        {
            return !(t1 == t2);
        }
    }
}
