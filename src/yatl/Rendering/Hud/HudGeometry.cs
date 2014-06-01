using System;
using amulware.Graphics;
using OpenTK;
using yatl.Utilities;
using Matrix2 = amulware.Graphics.Matrix2;

namespace yatl.Rendering.Hud
{
    class HudGeometry
    {
        private readonly Sprite2DGeometry barStart;
        private readonly Sprite2DGeometry barEnd;
        private readonly Sprite2DGeometry barMiddle;

        private readonly Sprite2DGeometry barThinStart;
        private readonly Sprite2DGeometry barThinEnd;
        private readonly Sprite2DGeometry barThinMiddle;

        private readonly Sprite2DGeometry barFilledStart;
        private readonly Sprite2DGeometry barFilledEnd;
        private readonly Sprite2DGeometry barFilledMiddle;

        private readonly Sprite2DGeometry square;
        private readonly Sprite2DGeometry squareFilled;

        private readonly IndexedSurface<UVColorVertexData> surface;

        public HudGeometry(SpriteSet<UVColorVertexData> sprites)
        {
            this.barStart = sprites["barStart"].Geometry as Sprite2DGeometry;
            this.barEnd = sprites["barEnd"].Geometry as Sprite2DGeometry;
            this.barMiddle = sprites["barMiddle"].Geometry as Sprite2DGeometry;

            this.barThinStart = sprites["barThinStart"].Geometry as Sprite2DGeometry;
            this.barThinEnd = sprites["barThinEnd"].Geometry as Sprite2DGeometry;
            this.barThinMiddle = sprites["barThinMiddle"].Geometry as Sprite2DGeometry;

            this.barFilledStart = sprites["barFilledStart"].Geometry as Sprite2DGeometry;
            this.barFilledEnd = sprites["barFilledEnd"].Geometry as Sprite2DGeometry;
            this.barFilledMiddle = sprites["barFilledMiddle"].Geometry as Sprite2DGeometry;

            this.square = sprites["square"].Geometry as Sprite2DGeometry;
            this.squareFilled = sprites["squarefilled"].Geometry as Sprite2DGeometry;

            this.surface = sprites.Surface;
        }

        public Color Color { get; set; }

        public void DrawSquare(Vector3 position, float angle = 0, float scale = 1, bool filled = false)
        {
            var geo = (filled ? this.squareFilled : this.square);

            geo.Color = this.Color;
            geo.DrawSprite(position, angle, scale);
        }

        public void DrawBarH(Vector3 position, float width, float height)
        {
            this.drawBarH(position, width, height, this.barStart, this.barMiddle, this.barEnd);
        }

        public void DrawThinBarH(Vector3 position, float width, float height)
        {
            this.drawBarH(position, width, height, this.barThinStart, this.barThinMiddle, this.barThinEnd);
        }

        public void DrawFilledBarH(Vector3 position, float width, float height)
        {
            this.drawBarH(position, width, height, this.barFilledStart, this.barFilledMiddle, this.barFilledEnd);
        }

        private void drawBarH(Vector3 position, float width, float height,
            Sprite2DGeometry start, Sprite2DGeometry middle, Sprite2DGeometry end)
        {
            if (width < 0)
            {
                position.X += width;
                width *= -1;
            }

            float endWidth = height * 0.25f;
            if (width < height * 0.5f)
            {
                endWidth = width * 0.5f;
                position.X += height * 0.025f;
            }
            else
            {
                middle.Color = this.Color;
                middle.DrawRectangle(position.X + endWidth, position.Y, position.Z, width - endWidth * 2, height);
            }

            start.Color = this.Color;
            start.DrawRectangle(position.X, position.Y, position.Z, endWidth, height);

            end.Color = this.Color;
            end.DrawRectangle(position.X + width - endWidth, position.Y, position.Z, endWidth, height);
        }

        public void DrawCircleSection(Vector3 center, Vector3 unitX, Vector3 unitY, float radius, float lineWidth,
            float startAngle, float angleSize, int fullCircleEdges = 32)
        {
            int edges = (int)Math.Ceiling(fullCircleEdges * Math.Abs(angleSize) / GameMath.TwoPi);

            int vertexCount = edges * 2 + 2;

            float angleStep = GameMath.TwoPi / fullCircleEdges;

            float innerRadius = radius - lineWidth;

            float outerRadiusFactor = radius / (radius - lineWidth);

            Vector2 inner = GameMath.Vector2FromRotation(startAngle, innerRadius);

            Matrix2 rotation = Matrix2.CreateRotation(angleStep);

            Vector2 uvInner = this.barFilledMiddle.UV.TopLeft;
            Vector2 uvOuter = this.barFilledMiddle.UV.BottomLeft;
            var argb = this.Color;

            var vertices = new UVColorVertexData[vertexCount];

            Vector2 outer = inner * outerRadiusFactor;
            vertices[0] = new UVColorVertexData(center + inner.X * unitX + inner.Y * unitY, uvInner, argb);
            vertices[1] = new UVColorVertexData(center + outer.X * unitX + outer.Y * unitY, uvOuter, argb);

            for (int i = 1; i < edges; i++)
            {
                inner = rotation * inner;
                outer = inner * outerRadiusFactor;

                vertices[i * 2] = new UVColorVertexData(center + inner.X * unitX + inner.Y * unitY, uvInner, argb);
                vertices[i * 2 + 1] = new UVColorVertexData(center + outer.X * unitX + outer.Y * unitY, uvOuter, argb);
            }

            inner = GameMath.Vector2FromRotation(startAngle + angleSize, innerRadius);
            outer = inner * outerRadiusFactor;
            vertices[vertexCount - 2] = new UVColorVertexData(center + inner.X * unitX + inner.Y * unitY, uvInner, argb);
            vertices[vertexCount - 1] = new UVColorVertexData(center + outer.X * unitX + outer.Y * unitY, uvOuter, argb);

            var offset = this.surface.AddVertices(vertices);

            var indices = new ushort[edges * 6];

            for (int i = 0; i < edges; i++)
            {
                int j = i * 6;
                ushort o = (ushort)(i * 2);
                indices[j] = (ushort)(offset + o);
                indices[j + 1] = (ushort)(offset + o + 2);
                indices[j + 2] = (ushort)(offset + o + 1);
                indices[j + 3] = (ushort)(offset + o + 1);
                indices[j + 4] = (ushort)(offset + o + 2);
                indices[j + 5] = (ushort)(offset + o + 3);
            }

            this.surface.AddIndices(indices);
        }
    }
}
