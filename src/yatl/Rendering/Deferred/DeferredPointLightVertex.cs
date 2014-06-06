using System.Runtime.InteropServices;
using amulware.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace yatl.Rendering.Deferred
{
    struct DeferredPointLightVertex : IVertexData
    {
        private Vector3 position;
        private Vector3 lightPosition;
        private float range;
        private Color color;
        private float intensity;

        public DeferredPointLightVertex(Vector3 position, Vector3 lightPosition, float range, Color color, float intensity)
        {
            this.position = position;
            this.lightPosition = lightPosition;
            this.range = range;
            this.color = color;
            this.intensity = intensity;
        }
        
        static private VertexAttribute[] vertexAttributes;

        private readonly static int size = Marshal.SizeOf(typeof(DeferredPointLightVertex));

        static private void setVertexAttributes()
        {
            DeferredPointLightVertex.vertexAttributes = new []{
                new VertexAttribute("v_position", 3, VertexAttribPointerType.Float, DeferredPointLightVertex.size, 0),
                new VertexAttribute("v_lightPosition", 3, VertexAttribPointerType.Float, DeferredPointLightVertex.size, 12),
                new VertexAttribute("v_lightRange", 1, VertexAttribPointerType.Float, DeferredPointLightVertex.size, 24),
                new VertexAttribute("v_lightColor", 4, VertexAttribPointerType.UnsignedByte, DeferredPointLightVertex.size, 28, true),
                new VertexAttribute("v_lightIntensity", 1, VertexAttribPointerType.Float, DeferredPointLightVertex.size, 32),
            };
        }

        public VertexAttribute[] VertexAttributes()
        {
            if (DeferredPointLightVertex.vertexAttributes == null)
                DeferredPointLightVertex.setVertexAttributes();
            return DeferredPointLightVertex.vertexAttributes;
        }

        public int Size()
        {
            return DeferredPointLightVertex.size;
        }
    }
}
