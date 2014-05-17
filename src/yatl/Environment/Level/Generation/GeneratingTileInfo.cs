using yatl.Environment.Tilemap.Hexagon;

namespace yatl.Environment.Level.Generation
{
    sealed class GeneratingTileInfo
    {
        public GeneratingTileInfo()
        {
        }

        public Directions OpenSides { get; set; }
        public bool Visited { get; set; }
    }
}
