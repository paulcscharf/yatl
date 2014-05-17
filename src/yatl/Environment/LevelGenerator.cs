using System.Linq;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Utilities;
using Direction = yatl.Environment.Tilemap.Hexagon.Direction;
using Extensions = yatl.Environment.Tilemap.Hexagon.Extensions;

namespace yatl.Environment
{
    sealed class LevelGenerator
    {
        private static readonly Direction[] activeDirections =
        {
            Direction.Left,
            Direction.DownLeft,
            Direction.DownRight,
        };

        public static LevelGenerator NewDefault { get { return new LevelGenerator().WithDefaultSettings; } }

        public int Radius { get; set; }
        public float Openness { get; set; }

        public LevelGenerator WithDefaultSettings
        {
            get
            {
                this.Radius = Settings.Game.Level.Radius;
                this.Openness = 0.4f;

                return this;
            }
        }

        public Tilemap<TileInfo> Generate()
        {
            var tempMap = new Tilemap<GeneratingTileInfo>(this.Radius);

            var tiles = tempMap.ToList();
            var tilesSpiral = tempMap.TilesSpiralOutward.ToList();
            var tilesDistance = tiles.OrderBy(
                t => (Settings.Game.Level.TileToPosition * t.Xy).LengthSquared
                ).ToList();

            foreach (var tile in tiles)
            {
                tempMap[tile] = new GeneratingTileInfo();
            }

            foreach (var tile in tiles)
            {
                var info = tile.Info;

                foreach (var direction in LevelGenerator.activeDirections)
                {
                    if (!(GlobalRandom.NextDouble() < this.Openness))
                        continue;

                    var other = tile.Neighbour(direction);
                    if (!other.IsValid)
                        continue;

                    info.OpenSides = info.OpenSides.And(direction);
                    other.Info.OpenSides = other.Info.OpenSides.And(direction.Opposite());
                }
            }

            return this.convertTempToMap(tempMap);
        }


        private Tilemap<TileInfo> convertTempToMap(Tilemap<GeneratingTileInfo> tempMap)
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
