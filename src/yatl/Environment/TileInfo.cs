using yatl.Environment.Tilemap.Hexagon;

namespace yatl.Environment
{
    sealed class TileInfo
    {
        public Directions OpenSides { get; private set; }

        public TileInfo(GeneratingTileInfo info)
        {
            this.OpenSides = info.OpenSides;
        }

    }
}
