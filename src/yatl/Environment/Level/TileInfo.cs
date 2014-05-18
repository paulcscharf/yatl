using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenTK;
using yatl.Environment.Level.Generation;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Utilities;
using Direction = yatl.Environment.Tilemap.Hexagon.Direction;
using Extensions = yatl.Environment.Tilemap.Hexagon.Extensions;

namespace yatl.Environment.Level
{
    sealed class TileInfo
    {
        public Directions OpenSides { get; private set; }

        public ReadOnlyCollection<Wall> Walls { get; private set; }

        public TileInfo(GeneratingTileInfo info)
        {
            this.OpenSides = info.OpenSides;

            this.generateWalls();
        }


        private void generateWalls()
        {
            var walls = new List<Wall>();


            if (this.OpenSides.Any())
            {
                var points = this.OpenSides.Enumerate()
                    .Select(TileInfo.makeWallPoints)
                    .ToList();

                Vector2 before = points.Last().Item2;
                var wallsToMake = points.Select(t =>
                    {
                        var w = new {From = before, To = t.Item1};
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

            var stepAngle = totalAngle / steps;

            var radiusStart = start.Length;
            var radiusEnd = end.Length;

            var before = start;

            for (int i = 1; i < steps; i++)
            {
                var f = (float)i / steps;

                var x = 1 - (float)Math.Sin(f * Math.PI);//2f * f - 1f;
                var y = x * x * 0.5f + 0.5f;

                y *= GlobalRandom.NextFloat(0.8f, 1.2f);

                var r = GameMath.Lerp(radiusStart, radiusEnd, f) * y;


                var point = (startDir + stepAngle * i).Vector * r;

                yield return new Wall(before, point);

                before = point;
            }

            yield return new Wall(before, end);
        }

        private static Tuple<Vector2, Vector2> makeWallPoints(Direction direction)
        {
            var corner1 = direction.CornerBefore() * Settings.Game.Level.HexagonSide;
            var corner2 = direction.CornerAfter() * Settings.Game.Level.HexagonSide;

            return Tuple.Create(
                Vector2.Lerp(corner1, corner2, 1 / 6f),
                Vector2.Lerp(corner1, corner2, 5 / 6f)
                );
        }
    }

    class Wall
    {
        public Vector2 StartPoint { get; private set; }
        public Vector2 EndPoint { get; private set; }

        public Wall(Vector2 startPoint, Vector2 endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }

    }
}
