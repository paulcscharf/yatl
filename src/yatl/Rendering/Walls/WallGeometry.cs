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
            var normal = wall.Normal;

            var start = wall.StartPoint + offset;
            var end = wall.EndPoint + offset;

            var before = wall.Previous;
            var after = wall.Next;

            var startNormal = new Vector3((normal + before.Normal).Normalized());
            var endNormal = new Vector3((normal + after.Normal).Normalized());

            var normal3D = new Vector3(normal);

            

            this.surface.AddQuad(
                new WallVertex(new Vector3(start.X, start.Y, 0), startNormal), // left bottom
                new WallVertex(new Vector3(start.X, start.Y, Settings.Game.Level.WallHeight) - startNormal, startNormal), // left top
                new WallVertex(new Vector3(end.X, end.Y, Settings.Game.Level.WallHeight) - endNormal, endNormal), // right top
                new WallVertex(new Vector3(end.X, end.Y, 0), endNormal) // right bottom
                );
        }
    }
}
