using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using yatl.Utilities;

namespace yatl.Environment.Tilemap.Hexagon
{
    static class Extensions
    {
        #region Lookup Tables

        private static readonly Step[] directionDelta =
        {
            new Step(0, 0),
            new Step(1, 0),
            new Step(0, 1),
            new Step(-1, 1),
            new Step(-1, 0),
            new Step(0, -1),
            new Step(1, -1),
        };
        private static readonly Direction[] directionOpposite =
        {
            Direction.Unknown,
            Direction.Right,
            Direction.UpRight,
            Direction.UpLeft,
            Direction.Left,
            Direction.DownLeft,
            Direction.DownRight,
        };

        public static readonly Direction[] Directions =
        {
            Direction.Left,
            Direction.DownLeft,
            Direction.DownRight,
            Direction.Right,
            Direction.UpRight,
            Direction.UpLeft,
        };

        private static readonly Vector2[] corners =
            Enumerable.Range(0, 7).Select(i => GameMath.Vector2FromRotation(
                (i * 60f - 30f).Degrees().Radians))
                .ToArray();

        #endregion

        #region Tile<TTileInfo>

        public static IEnumerable<Tile<TTileInfo>> PossibleNeighbours<TTileInfo>(this Tile<TTileInfo> tile)
        {
            yield return tile.Neighbour(Extensions.directionDelta[1]);
            yield return tile.Neighbour(Extensions.directionDelta[2]);
            yield return tile.Neighbour(Extensions.directionDelta[3]);
            yield return tile.Neighbour(Extensions.directionDelta[4]);
            yield return tile.Neighbour(Extensions.directionDelta[5]);
            yield return tile.Neighbour(Extensions.directionDelta[6]);
        }

        #endregion

        #region Direction and Directions

        public static IEnumerable<Direction> Enumerate(this Directions directions)
        {
            return Extensions.Directions.Where(direction => directions.Includes(direction));
        }

        public static Vector2 CornerBefore(this Direction direction)
        {
            if(direction == Direction.Unknown)
                throw new ArgumentOutOfRangeException("direction", "Cannot get corner for unknown direction.");

            return Extensions.corners[(int)direction - 1];
        }

        public static Vector2 CornerAfter(this Direction direction)
        {
            if (direction == Direction.Unknown)
                throw new ArgumentOutOfRangeException("direction", "Cannot get corner for unknown direction.");

            return Extensions.corners[(int)direction];
        } 

        public static Step Step(this Direction direction)
        {
            return Extensions.directionDelta[(int)direction];
        }

        public static bool Any(this Directions direction)
        {
            return direction != Hexagon.Directions.None;
        }
        public static bool Any(this Directions direction, Directions match)
        {
            return direction.Intersect(match) != Hexagon.Directions.None;
        }

        public static bool All(this Directions direction)
        {
            return direction == Hexagon.Directions.All;
        }
        public static bool All(this Directions direction, Directions match)
        {
            return direction.Intersect(match) == match;
        }

        public static Direction Hexagonal(this Utilities.Direction direction)
        {
            return (Direction)((int)Math.Floor(direction.Degrees * 1 / 60f + 0.5f) % 6 + 1);
        }

        public static Direction Opposite(this Direction direction)
        {
            return Extensions.directionOpposite[(int)direction];
        }


        private static Directions toDirections(this Direction direction)
        {
            return (Directions)(1 << ((int)direction - 1));
        }

        public static bool Includes(this Directions directions, Direction direction)
        {
            return directions.HasFlag(direction.toDirections());
        }

        public static Directions And(this Directions directions, Direction direction)
        {
            return directions | direction.toDirections();
        }

        public static Directions Except(this Directions directions, Direction direction)
        {
            return directions & ~direction.toDirections();
        }

        public static Directions Union(this Directions directions, Directions directions2)
        {
            return directions | directions2;
        }

        public static Directions Except(this Directions directions, Directions directions2)
        {
            return directions & ~directions2;
        }

        public static Directions Intersect(this Directions directions, Directions directions2)
        {
            return directions & directions2;
        }

        public static bool IsSubsetOf(this Directions directions, Directions directions2)
        {
            return directions2.HasFlag(directions);
        }

        #endregion

        public static int TileCountForRadius(int radius)
        {
            int dim = radius * 2 + 1;
            return (dim * dim * 3) / 4;
        }
    }
}
