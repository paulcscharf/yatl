using System;
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

        public static void MakeFloors(this IEnumerable<GeneratingTile> tiles)
        {
            foreach (var tile in tiles)
                tile.Info.GenerateFloor();
        }

        public static void MakeWalls(this IEnumerable<GeneratingTile> tiles)
        {
            foreach (var tile in tiles)
                tile.Info.GenerateWalls();
            foreach (var tile in tiles)
                tile.connectWallsToNeighbours();
        }

        private static void connectWallsToNeighbours(this GeneratingTile tile)
        {
            var info = tile.Info;
            foreach (var tuple in info.OpenSides.Enumerate()
                .Select(d => new { Direction = d, Neighbour = tile.Neighbour(d)})
                .Where(t => t.Neighbour.IsValid))
            {
                info.ConnectEdgeWalls(tuple.Neighbour.Info, tuple.Direction);
            }
        }

        public static void OpenRandomSpanningTree(this GeneratingTile tile, float minOpen, float maxOpen)
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
                tile.OpenTileWall(direction, minOpen, maxOpen);
                next.OpenRandomSpanningTree(minOpen, maxOpen);
            }
        }

        public static void OpenRandomWalls(this IEnumerable<GeneratingTile> tiles, float percentage, float minOpen, float maxOpen)
        {
            tiles.OpenRandomWalls(t => percentage, minOpen, maxOpen);
        }

        public static void OpenRandomWalls(this IEnumerable<GeneratingTile> tiles, Func<GeneratingTile, float> probability, float minOpen, float maxOpen)
        {
            foreach (var tile in tiles)
            {
                // ReSharper disable once AccessToForEachVariableInClosure
                foreach (var direction in Helpers.activeDirections
                    .Where(d => GlobalRandom.NextDouble() < probability(tile)))
                {
                    tile.OpenTileWall(direction, minOpen, maxOpen);
                }
            }
        }

        public static bool OpenTileWall(this Tile<GeneratingTileInfo> tile, Direction direction, float minOpen, float maxOpen)
        {
            return tile.OpenTileWall(direction, GlobalRandom.NextFloat(minOpen, maxOpen));
        }

        public static bool OpenTileWall(this Tile<GeneratingTileInfo> tile, Direction direction, float width)
        {
            var other = tile.Neighbour(direction);
            if (!other.IsValid)
                return false;

            var info = tile.Info;
            var info2 = other.Info;

            var oppositeDir = direction.Opposite();

            info.OpenSides = info.OpenSides.And(direction);
            info2.OpenSides = info2.OpenSides.And(oppositeDir);

            info.CorridorWidths[(int)direction] = width;
            info2.CorridorWidths[(int)oppositeDir] = width;

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
