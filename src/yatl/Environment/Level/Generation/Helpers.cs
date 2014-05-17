using System.Collections.Generic;
using System.Linq;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Utilities;
using Direction = yatl.Environment.Tilemap.Hexagon.Direction;
using Extensions = yatl.Environment.Tilemap.Hexagon.Extensions;
using GeneratingTile = yatl.Environment.Tilemap.Hexagon.Tile<yatl.Environment.Level.Generation.GeneratingTileInfo>;

namespace yatl.Environment.Level.Generation
{
    static class Helpers
    {
        private static readonly Direction[] activeDirections =
        {
            Direction.Left,
            Direction.DownLeft,
            Direction.DownRight,
        };

        public static void OpenRandomSpanningTree(this GeneratingTile tile)
        {
            tile.Info.Visited = true;
            var directions = Extensions.Directions.Shuffled();

            foreach (var direction in directions)
            {
                var next = tile.Neighbour(direction);
                if (!next.IsValid)
                    continue;
                if (next.Info.Visited)
                    continue;
                tile.OpenTileWall(direction);
                next.OpenRandomSpanningTree();
            }
        }

        public static void OpenRandomWalls(this IEnumerable<GeneratingTile> tiles, float percentage)
        {
            foreach (var tile in tiles)
            {
                foreach (var direction in Helpers.activeDirections
                    .Where(d => GlobalRandom.NextDouble() < percentage))
                {
                    tile.OpenTileWall(direction);
                }
            }
        }

        public static bool OpenTileWall(this Tile<GeneratingTileInfo> tile, Direction direction)
        {
            var other = tile.Neighbour(direction);
            if (!other.IsValid)
                return false;

            var info = tile.Info;
            var info2 = other.Info;

            info.OpenSides = info.OpenSides.And(direction);
            info2.OpenSides = info2.OpenSides.And(direction.Opposite());

            return true;
        }


        public static Tilemap<TileInfo> Build(this Tilemap<GeneratingTileInfo> tempMap)
        {
            var map = new Tilemap<TileInfo>(tempMap.Radius);

            foreach (var tile in tempMap)
            {
                map[tile] = new TileInfo(tile.Info);
            }

            return map;
        }
    }
}
