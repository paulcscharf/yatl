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

                walls.AddRange(wallsToMake.Select(w => new Wall(w.From, w.To)));

            }


            this.Walls = walls.AsReadOnly();
        }

        private static Tuple<Vector2, Vector2> makeWallPoints(Direction direction)
        {
            var corner1 = direction.CornerBefore() * Settings.Game.Level.HexagonSide;
            var corner2 = direction.CornerAfter() * Settings.Game.Level.HexagonSide;

            return Tuple.Create(
                Vector2.Lerp(corner1, corner2, 1 / 3f),
                Vector2.Lerp(corner1, corner2, 2 / 3f)
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
