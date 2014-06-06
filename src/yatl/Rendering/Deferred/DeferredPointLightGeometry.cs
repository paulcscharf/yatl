using amulware.Graphics;
using OpenTK;

namespace yatl.Rendering.Deferred
{
    sealed class DeferredPointLightGeometry
    {
        private readonly IndexedSurface<DeferredPointLightVertex> surface;

        public DeferredPointLightGeometry(IndexedSurface<DeferredPointLightVertex> surface)
        {
            this.surface = surface;
        }

        public void Draw(Vector3 position, Color color, float intensity, float range)
        {
            var left = position.X - range;
            var right = position.X + range;
            var top = position.Y + range;
            var bottom = position.Y - range;

            var floor = 0f;
            var ceiling = 2f;

            var index = this.surface.AddVertices(
                new DeferredPointLightVertex(new Vector3(left, top, ceiling), position, range, color, intensity),
                new DeferredPointLightVertex(new Vector3(right, top, ceiling), position, range, color, intensity),
                new DeferredPointLightVertex(new Vector3(left, bottom, ceiling), position, range, color, intensity),
                new DeferredPointLightVertex(new Vector3(right, bottom, ceiling), position, range, color, intensity),

                new DeferredPointLightVertex(new Vector3(left, top, floor), position, range, color, intensity),
                new DeferredPointLightVertex(new Vector3(right, top, floor), position, range, color, intensity),
                new DeferredPointLightVertex(new Vector3(left, bottom, floor), position, range, color, intensity),
                new DeferredPointLightVertex(new Vector3(right, bottom, floor), position, range, color, intensity)
                );

            /*
             * 0---1
             * |   |
             * 2---3
             * 
             * 4---5
             * |   |
             * 6---7
             * 
             */

            var offsets = new ushort[]
            {
                0, 1, 2,
                1, 3, 2,

                0, 4, 1,
                1, 4, 5,

                1, 5, 3,
                5, 7, 3,

                3, 7, 2,
                7, 6, 2,
                
                0, 2, 6,
                6, 4, 0
            };

            for (int i = 0; i < offsets.Length; i++)
            {
                offsets[i] += index;
            }

            this.surface.AddIndices(offsets);
        }
    }
}
