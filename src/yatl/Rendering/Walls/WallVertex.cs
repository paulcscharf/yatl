using amulware.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace yatl.Rendering.Walls
{
    struct WallVertex : IVertexData
    {
        private readonly Vector3 position;
        private readonly Vector3 normal;

        public WallVertex(Vector3 position, Vector3 normal)
        {
            this.position = position;
            this.normal = normal;
        }

        static private void setVertexAttributes()
        {
            WallVertex.vertexAttributes = new[]{
                new VertexAttribute("v_position", 3, VertexAttribPointerType.Float, WallVertex.size, 0),
                new VertexAttribute("v_normal", 3, VertexAttribPointerType.Float, WallVertex.size, 12),
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
