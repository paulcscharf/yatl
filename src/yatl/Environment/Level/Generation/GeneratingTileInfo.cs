using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Utilities;
using Direction = yatl.Environment.Tilemap.Hexagon.Direction;
using Hex = yatl.Settings.Game.Level;

namespace yatl.Environment.Level.Generation
{
    sealed class GeneratingTileInfo
    {
        public GeneratingTileInfo()
        {
            this.CorridorWidths = new float[7];
        }

        public Directions OpenSides { get; set; }
        public bool Visited { get; set; }

        public float[] CorridorWidths { get; private set; }

        public List<Wall> Walls { get; private set; }
        public TriangulatedFloor Floor { get; private set; }

        public float Lightness { get; set; }

        private Dictionary<Direction, EdgeWallPair> edgeWalls;

        public class EdgeWallPair
        {
            private readonly Wall wallIn;
            private readonly Wall wallOut;

            public Wall WallIn { get { return this.wallIn; } }
            public Wall WallOut { get { return this.wallOut; } }

            public EdgeWallPair(Wall wallIn, Wall wallOut)
            {
                this.wallIn = wallIn;
                this.wallOut = wallOut;
            }

            public void ConnectWalls(EdgeWallPair other)
            {
                this.wallIn.Previous = other.wallOut;
                this.wallOut.Next = other.wallIn;
                other.wallIn.Previous = this.wallOut;
                other.wallOut.Next = this.wallIn;
            }
        }

        public void GenerateFloor()
        {
            this.Floor = new TriangulatedFloor(this.OpenSides, this.edgeWalls);
        }

        #region generate walls

        public void ConnectEdgeWalls(GeneratingTileInfo other, Direction directionToOther)
        {
            var edges = this.edgeWalls[directionToOther];
            var otherEdges = other.edgeWalls[directionToOther.Opposite()];

            edges.ConnectWalls(otherEdges);
        }

        public void GenerateWalls()
        {
            this.Walls = new List<Wall>();

            this.edgeWalls = new Dictionary<Direction, EdgeWallPair>();

            var wallsIn = new Dictionary<Direction, Wall>();
            var wallsOut = new Dictionary<Direction, Wall>();

            if (this.OpenSides.Any())
            {
                var edges = this.OpenSides.Enumerate()
                    .Select(d => new
                    {
                        Direction = d,
                        Points = GeneratingTileInfo.makeWallPoints(d, this.CorridorWidths[(int)d])
                    }).ToList();

                for (int i = 0; i < edges.Count; i++)
                {
                    var from = edges[i];
                    var to = edges[(i + 1) % edges.Count];

                    var walls = GeneratingTileInfo.makeWallSections(from.Points.Item2, to.Points.Item1).ToList();

                    wallsIn[from.Direction] = walls.First();
                    wallsOut[to.Direction] = walls.Last();

                    this.Walls.AddRange(walls);
                }


                foreach (var direction in this.OpenSides.Enumerate())
                {
                    this.edgeWalls[direction] = new EdgeWallPair(wallsIn[direction], wallsOut[direction]);
                }
            }
        }

        private static IEnumerable<Wall> makeWallSections(Vector2 start, Vector2 end)
        {
            var startDir = Utilities.Direction.Of(start);
            var endDir = Utilities.Direction.Of(end);

            var totalAngle = endDir - startDir;

            if (totalAngle.Radians < 0)
                totalAngle += 360f.Degrees();

            var steps = Math.Max(3, (int)(totalAngle.MagnitudeInDegrees / 20));

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
            Wall lastWall = null;

            for (int i = 1; i < steps; i++)
            {
                var f = (float)i / steps;

                var point = bezier.CalculatePoint(f);

                var wall = new Wall(before, point);

                if (lastWall != null)
                {
                    wall.Previous = lastWall;
                    lastWall.Next = wall;
                }

                yield return wall;

                lastWall = wall;
                before = point;
            }

            var endWall = new Wall(before, end);

            if (lastWall != null)
            {
                endWall.Previous = lastWall;
                lastWall.Next = endWall;
            }

            yield return endWall;
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

    }
}
