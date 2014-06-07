using amulware.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace yatl.Rendering.Walls
{
    struct WallVertex : IVertexData
    {
        private readonly Vector3 position;
        private readonly Vector3 normal00;
        private readonly Vector3 normal10;
        private readonly Vector3 normal01;
        private readonly Vector3 normal11;
        private readonly Vector2 normalUV;

        public WallVertex(Vector3 position, Vector3 normal)
        {
            this.position = position;
            this.normal00 = normal;
            this.normal10 = normal;
            this.normal01 = normal;
            this.normal11 = normal;
            this.normalUV = Vector2.Zero;
        }

        public WallVertex(Vector3 position, Vector3 normal00, Vector3 normal10, Vector3 normal01, Vector3 normal11, float normalU, float normalV)
        {
            this.position = position;
            this.normal00 = normal00;
            this.normal10 = normal10;
            this.normal01 = normal01;
            this.normal11 = normal11;
            this.normalUV = new Vector2(normalU, normalV);
        }

        static private void setVertexAttributes()
        {
            WallVertex.vertexAttributes = new[]{
                new VertexAttribute("v_position", 3, VertexAttribPointerType.Float, WallVertex.size, 0),
                new VertexAttribute("v_normal00", 3, VertexAttribPointerType.Float, WallVertex.size, 12),
                new VertexAttribute("v_normal10", 3, VertexAttribPointerType.Float, WallVertex.size, 24),
                new VertexAttribute("v_normal01", 3, VertexAttribPointerType.Float, WallVertex.size, 36),
                new VertexAttribute("v_normal11", 3, VertexAttribPointerType.Float, WallVertex.size, 48),
                new VertexAttribute("v_normalUV", 2, VertexAttribPointerType.Float, WallVertex.size, 60),
            };
        }

        #region book keeping

        private static VertexAttribute[] vertexAttributes;

        private static readonly int size = Marshal.SizeOf(typeof(WallVertex));

        public VertexAttribute[] VertexAttributes()
        {
            if (WallVertex.vertexAttributes == null)
                WallVertex.setVertexAttributes();
            return WallVertex.vertexAttributes;
        }

        public int Size()
        {
            return WallVertex.size;
        }

        #endregion

    }
}
