using amulware.Graphics;
using OpenTK;
using yatl.Environment.Level;
using yatl.Environment.Level.Generation;
using yatl.Utilities;

namespace yatl.Rendering.Walls
{
    sealed class WallGeometry
    {
        private readonly IndexedSurface<WallVertex> surface;

        public WallGeometry(IndexedSurface<WallVertex> surface)
        {
            this.surface = surface;
        }

        public void DrawFloor(TriangulatedFloor floor, Vector2 offset)
        {
            floor.AddToSurface(this.surface, offset);
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

            var startTangent = startNormal.Xy.PerpendicularLeft.WithZ(0);
            var endTangent = endNormal.Xy.PerpendicularLeft.WithZ(0);
            var startUp = (new Vector3(0, 0, Settings.Game.Level.WallHeight) - startNormal);
            var endUp = (new Vector3(0, 0, Settings.Game.Level.WallHeight) - endNormal);

            var startNormal3D = Vector3.Cross(startTangent, startUp).Normalized();
            var endNormal3D = Vector3.Cross(endTangent, endUp).Normalized();

            var normal00 = Vector3.UnitZ; // left bottom
            var normal10 = Vector3.UnitZ; // right bottom
            var normal01 = startNormal3D; // left top
            var normal11 = endNormal3D; // right top

            this.surface.AddQuad(
                new WallVertex(new Vector3(start.X, start.Y, 0),
                    normal00, normal10, normal01, normal11, 0, 0), // left bottom

                new WallVertex(new Vector3(start.X, start.Y, Settings.Game.Level.WallHeight) - startNormal,
                    normal00, normal10, normal01, normal11, 0, 1), // left top

                new WallVertex(new Vector3(end.X, end.Y, Settings.Game.Level.WallHeight) - endNormal,
                    normal00, normal10, normal01, normal11, 1, 1), // right top

                new WallVertex(new Vector3(end.X, end.Y, 0),
                    normal00, normal10, normal01, normal11, 1, 0) // right bottom
                );
        }
    }
}
