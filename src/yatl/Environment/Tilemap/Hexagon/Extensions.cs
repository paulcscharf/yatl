using System.Collections.Generic;

namespace yatl.Environment.Tilemap.Hexagon
{
    static class Extensions
    {
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

        public static Step Step(this Direction direction)
        {
            return Extensions.directionDelta[(int)direction];
        }


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
    }
}
