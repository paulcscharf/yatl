using OpenTK;

namespace yatl.Environment.Level
{
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
    }
}