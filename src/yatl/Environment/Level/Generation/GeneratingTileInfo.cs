using yatl.Environment.Tilemap.Hexagon;

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
    }
}
