using amulware.Graphics;
using OpenTK;
using yatl.Environment.Level;

namespace yatl.Rendering.Walls
{
    sealed class WallGeometry
    {
        private readonly IndexedSurface<WallVertex> surface;

        public WallGeometry(IndexedSurface<WallVertex> surface)
        {
            this.surface = surface;
        }

        public void DrawWall(Wall wall, Vector2 offset)
        {
            var normal = new Vector3((wall.EndPoint - wall.StartPoint).PerpendicularLeft.Normalized());

            var start = wall.StartPoint + offset;
            var end = wall.EndPoint + offset;

            this.surface.AddQuad(
                new WallVertex(new Vector3(start.X, start.Y, 0), normal), // left bottom
                new WallVertex(new Vector3(start.X, start.Y, Settings.Game.Level.WallHeight) - normal, normal), // left top
                new WallVertex(new Vector3(end.X, end.Y, Settings.Game.Level.WallHeight) - normal, normal), // right top
                new WallVertex(new Vector3(end.X, end.Y, 0), normal) // right bottom
                );
        }
    }
}
