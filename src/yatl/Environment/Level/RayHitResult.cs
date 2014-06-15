using OpenTK;
using yatl.Environment.Tilemap.Hexagon;

namespace yatl.Environment.Level
{
    struct TiledRayHitResult
    {
        public readonly Tile<TileInfo> Tile;
        public readonly Vector2 GlobalPoint;
        public readonly RayHitResult Results;

        public TiledRayHitResult(Tile<TileInfo> tile, RayHitResult results, Vector2 tileOffset)
        {
            this.Tile = tile;
            this.GlobalPoint = results.Point + tileOffset;
            this.Results = results;
        }

    }

    struct RayHitResult
    {
        public readonly bool Hit;
        public readonly float RayFactor;
        public readonly Vector2 Point;
        public readonly Vector2 Normal;

        public RayHitResult(bool hit, float rayFactor, Vector2 point, Vector2 normal)
        {
            this.Hit = hit;
            this.RayFactor = rayFactor;
            this.Point = point;
            this.Normal = normal;
        }

        public RayHitResult WithNewPoint(Vector2 point)
        {
            return new RayHitResult(this.Hit, this.RayFactor, point, this.Normal);
        }

        public TiledRayHitResult OnTile(Tile<TileInfo> tile, Vector2 offset)
        {
            return new TiledRayHitResult(tile, this, offset);
        }
    }
}
