using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using amulware.Graphics;
using OpenTK;
using yatl.Environment.Level.Generation;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Rendering;
using yatl.Rendering.Walls;
using Hex = yatl.Settings.Game.Level;

namespace yatl.Environment.Level
{
    sealed class TileInfo
    {
        public Directions OpenSides { get; private set; }

        public ReadOnlyCollection<Wall> Walls { get; private set; }

        public TriangulatedFloor Floor { get; private set; }


        private IndexedSurface<WallVertex> geometry;

        public TileInfo(GeneratingTileInfo info)
        {
            this.OpenSides = info.OpenSides;

            this.Walls = (info.Walls ?? Enumerable.Empty<Wall>())
                .Select(w => w.Frozen).ToList().AsReadOnly();

            this.Floor = info.Floor;

        }

        public void InitGeometry(Vector2 offset)
        {
            this.DisposeGeometry();

            this.geometry = SurfaceManager.Instance.MakeLevelGeometrySurface();
            var geo = new WallGeometry(this.geometry);
            foreach (var wall in this.Walls)
                geo.DrawWall(wall, offset);
            geo.DrawFloor(this.Floor, offset);
        }

        public void DisposeGeometry()
        {
            if (this.geometry == null)
                return;
            SurfaceManager.Instance.DisposeOfLevelGeometrySurface(this.geometry);
            this.geometry = null;
        }

        public RayHitResult ShootRay(Ray ray)
        {
            var result = new RayHitResult(false, 1, ray.Start + ray.Direction, Vector2.Zero);

            foreach (var wall in this.Walls)
            {
                var wS = wall.StartPoint;
                var wD = wall.EndPoint - wS;

                var denominator = ray.Direction.Y * wD.X - ray.Direction.X * wD.Y;

                // disregard backfacing and parallel walls
                // (denominator is dot product of non-unit normal and ray)
                if (denominator >= 0)
                    continue;

                var numerator = (wS.Y - ray.Start.Y) * wD.X + (ray.Start.X - wS.X) * wD.Y;

                var f = numerator / denominator;

                // disregard behind and further than previous result
                if (f < -0.001f || f > result.RayFactor)
                    continue;

                var point = ray.Start + f * ray.Direction;

                var wF = wD.X != 0
                    ? (point.X - wS.X) / wD.X
                    : (point.Y - wS.Y) / wD.Y;

                // disregard outside of line segment
                if (wF < 0 || wF > 1)
                    continue;

                result = new RayHitResult(true, f, point, wall.Normal);
            }

            return result;
        }

        public void Draw()
        {
            SurfaceManager.Instance.QueueLevelGeometry(this.geometry);
        }
    }
}
