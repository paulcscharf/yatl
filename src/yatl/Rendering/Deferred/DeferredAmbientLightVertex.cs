using System.Runtime.InteropServices;
using amulware.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace yatl.Rendering.Deferred
{
    struct DeferredAmbientLightVertex : IVertexData
    {
        private Vector3 position;
        private Color color;

        public DeferredAmbientLightVertex(Vector3 position, Color color)
        {
            this.position = position;
            this.color = color;
        }

        static private VertexAttribute[] vertexAttributes;

        private readonly static int size = Marshal.SizeOf(typeof(DeferredAmbientLightVertex));

        static private void setVertexAttributes()
        {
            DeferredAmbientLightVertex.vertexAttributes = new[]{
                new VertexAttribute("v_position", 3, VertexAttribPointerType.Float, DeferredAmbientLightVertex.size, 0),
                new VertexAttribute("v_color", 4, VertexAttribPointerType.Byte, DeferredAmbientLightVertex.size, 12, true),
            };
        }

        public VertexAttribute[] VertexAttributes()
        {
            if (DeferredAmbientLightVertex.vertexAttributes == null)
                DeferredAmbientLightVertex.setVertexAttributes();
            return DeferredAmbientLightVertex.vertexAttributes;
        }

        public int Size()
        {
            return DeferredAmbientLightVertex.size;
        }
    }
}
