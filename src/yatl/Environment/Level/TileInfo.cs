using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenTK;
using yatl.Environment.Level.Generation;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Utilities;
using Direction = yatl.Environment.Tilemap.Hexagon.Direction;
using Hex = yatl.Settings.Game.Level;

namespace yatl.Environment.Level
{
    sealed class TileInfo
    {
        public Directions OpenSides { get; private set; }

        public ReadOnlyCollection<Wall> Walls { get; private set; }

        public TileInfo(GeneratingTileInfo info)
        {
            this.OpenSides = info.OpenSides;

            this.generateWalls(info);
        }

        #region initialising

        private void generateWalls(GeneratingTileInfo info)
        {
            var walls = new List<Wall>();


            if (this.OpenSides.Any())
            {
                var points = this.OpenSides.Enumerate()
                    .Select(d => TileInfo.makeWallPoints(d, info.CorridorWidths[(int)d]))
                    .ToList();

                Vector2 before = points.Last().Item2;
                var wallsToMake = points.Select(t =>
                {
                    var w = new { From = before, To = t.Item1 };
                    before = t.Item2;
                    return w;
                });

                walls.AddRange(wallsToMake.SelectMany(w => TileInfo.makeWallSections(w.From, w.To)));

            }


            this.Walls = walls.AsReadOnly();
        }

        private static IEnumerable<Wall> makeWallSections(Vector2 start, Vector2 end)
        {
            var startDir = Utilities.Direction.Of(start);
            var endDir = Utilities.Direction.Of(end);

            var totalAngle = endDir - startDir;

            if (totalAngle.Radians < 0)
                totalAngle += 360f.Degrees();

            var steps = Math.Max(3, (int)(totalAngle.MagnitudeInDegrees / 30));

            var radiusStart = start.Length;
            var radiusEnd = end.Length;

            var points = steps > 3
                ? new[]
                {
                    start,
                    start * 0.8f,
                    (startDir + totalAngle * 0.3f).Vector * radiusStart * 0.15f * steps,
                    (startDir + totalAngle * 0.7f).Vector * radiusEnd * 0.15f * steps,
                    end * 0.8f,
                    end
                }
                : new[]
                {
                    start,
                    start * 0.8f,
                    end * 0.8f,
                    end
                };

            var bezier = new BezierCurve(points);

            var before = start;

            for (int i = 1; i < steps; i++)
            {
                var f = (float)i / steps;

                var point = bezier.CalculatePoint(f);

                yield return new Wall(before, point);

                before = point;
            }

            yield return new Wall(before, end);
        }

        private static Tuple<Vector2, Vector2> makeWallPoints(Direction direction, float width)
        {
            var corner1 = direction.CornerBefore() * Settings.Game.Level.HexagonSide;
            var corner2 = direction.CornerAfter() * Settings.Game.Level.HexagonSide;

            float halfWidth = width * 0.5f;

            return Tuple.Create(
                Vector2.Lerp(corner1, corner2, 0.5f - halfWidth),
                Vector2.Lerp(corner1, corner2, 0.5f + halfWidth)
                );
        }
        #endregion


        public RayHitResult ShootRay(Ray ray)
        {
            var result = new RayHitResult(false, 1, ray.Start + ray.Direction, Vector2.Zero);

            foreach (var wall in this.Walls)
            {
                var wS = wall.StartPoint;
                var wD = wall.EndPoint - wS;

                var denominator = ray.Direction.Y * wD.X - ray.Direction.X * wD.Y;

                // disregard backfacing and parallel walls
                // (denominator is dot product of normal and ray)
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

                result = new RayHitResult(true, f, point, wD.PerpendicularLeft.Normalized());
            }

            return result;
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
    }

    struct Ray
    {
        public readonly Vector2 Start;
        public readonly Vector2 Direction;

        public Ray(Vector2 start, Vector2 direction)
        {
            this.Start = start;
            this.Direction = direction;
        }
    }
}
