using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using amulware.Graphics;
using OpenTK;
using OpenTK.Audio.OpenAL;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Rendering.Walls;
using yatl.Utilities;
using Direction = yatl.Environment.Tilemap.Hexagon.Direction;

namespace yatl.Environment.Level.Generation
{
    sealed class TriangulatedFloor
    {
        private Vector2[] vertices;
        private Triangle[] triangles;

        public TriangulatedFloor(Directions openSides, IDictionary<Direction, GeneratingTileInfo.EdgeWallPair> edgeWalls)
        {
            this.makeVertices(openSides, edgeWalls);
            this.triangulate();
        }

        private void makeVertices(Directions openSides, IDictionary<Direction, GeneratingTileInfo.EdgeWallPair> walls)
        {
            var sides = openSides.Enumerate().ToList();

            // add first again to avoid easily handle wrapping
            sides.Add(sides[0]);

            var vertices = new List<Vector2>();

            for (int i = 0; i < sides.Count - 1; i++)
            {
                // make vertices connecting side i to side i + 1

                var wallFrom = walls[sides[i]].WallIn;
                var wallTo = walls[sides[i + 1]].WallOut;

                vertices.Add(wallFrom.StartPoint);
                while (true)
                {
                    vertices.Add(wallFrom.EndPoint);
                    if (wallFrom == wallTo)
                        break;
                    wallFrom = wallFrom.Next;
                }
            }

            this.vertices = vertices.ToArray();
        }

        private void triangulate()
        {
            var triangles = new List<Triangle>(this.vertices.Length - 2);

            // annotated vertices know if they are convex (== ears)
            var annotatedVertices = this.vertices.Select((v, i) =>
                new AnnotatedVertex((ushort)i,
                    this.getVertex(i), this.getVertex(i - 1), this.getVertex(i + 1)
                    )).ToList();

            int currentId = 0;


            while (annotatedVertices.Count > 3)
            {
                int beforeId;
                int afterId;

                // find ear
                int verticesChecked = 0;
                while (true)
                {
                    currentId = (currentId + 1) % annotatedVertices.Count;
                    beforeId = (currentId - 1 + annotatedVertices.Count) % annotatedVertices.Count;
                    afterId = (currentId + 1) % annotatedVertices.Count;

                    // break if current vertex is ear (convex with empty triangle)
                    if (annotatedVertices[currentId].IsConvex)
                    {
                        bool isEar = true;

                        // or you know... this works too...
                        var v0 = annotatedVertices[currentId].Position;

                        var v1 = annotatedVertices[beforeId].Position - v0;
                        var v2 = annotatedVertices[afterId].Position - v0;

                        for (int i = (afterId + 1) % annotatedVertices.Count;
                            i != beforeId; i = (i + 1) % annotatedVertices.Count)
                        {
                            var p = this.vertices[annotatedVertices[i].Id] - v0;
                            if (TriangulatedFloor.isInsideTriangle(p, v1, v2))
                            {
                                isEar = false;
                                break;
                            }
                        }

                        if (isEar)
                            break;
                    }

                    verticesChecked++;
                    if (verticesChecked > annotatedVertices.Count)
                        throw new Exception("Tried to triangulate polygon, but could not find ear to cut.");
                }

                var before = annotatedVertices[beforeId];
                var current = annotatedVertices[currentId];
                var after = annotatedVertices[afterId];

                // add triangle
                triangles.Add(new Triangle(
                    before.Id, current.Id, after.Id)
                    );

                // update adjoining vertices and cut ear
                before.SetAfter(this.vertices[after.Id]);
                after.SetBefore(this.vertices[before.Id]);
                annotatedVertices.RemoveAt(currentId);
            }

            // add last triangle
            triangles.Add(new Triangle(annotatedVertices[0].Id,
                annotatedVertices[1].Id, annotatedVertices[2].Id));

            this.triangles = triangles.ToArray();
        }

        private static bool isInsideTriangle(Vector2 point, Vector2 v1, Vector2 v2)
        {
            float denominator = (v1.Y * v2.X - v1.X * v2.Y);

            if (denominator == 0)
                throw new Exception();

            float denominatorInv = 1 / denominator;

            float a = (v1.Y * point.X - v1.X * point.Y) * denominatorInv;
            if (a < 0)
                return false;

            float b = (v2.X * point.Y - v2.Y * point.X) * denominatorInv;
            if (b < 0 || a + b > 1)
                return false;

            return true;
        }

        private Vector2 getVertex(int i)
        {
            return this.vertices[(i + this.vertices.Length) % this.vertices.Length];
        }

        private class AnnotatedVertex
        {
            public readonly ushort Id;
            public readonly Vector2 Position;

            private bool isConvex;

            private Utilities.Direction toBefore;
            private Utilities.Direction toAfter;

            public AnnotatedVertex(ushort id, Vector2 position, Vector2 before, Vector2 after)
            {
                this.Id = id;
                this.Position = position;
                toBefore = Utilities.Direction.Of(before - position);
                toAfter = Utilities.Direction.Of(after - position);
                this.updateConvex();
            }

            private void updateConvex()
            {
                this.isConvex = (this.toBefore - this.toAfter).Radians > 0;
            }

            public void SetBefore(Vector2 before)
            {
                toBefore = Utilities.Direction.Of(before - this.Position);
                this.updateConvex();
            }

            public void SetAfter(Vector2 after)
            {
                toAfter = Utilities.Direction.Of(after - this.Position);
                this.updateConvex();
            }

            public bool IsConvex { get { return this.isConvex; } }
        }

        private struct Triangle
        {
            public readonly ushort Id0;
            public readonly ushort Id1;
            public readonly ushort Id2;

            public Triangle(ushort id0, ushort id1, ushort id2)
            {
                this.Id0 = id0;
                this.Id1 = id1;
                this.Id2 = id2;
            }
        }
        
        public void AddToSurface(IndexedSurface<WallVertex> surface, Vector2 offset)
        {
            var vertices = new WallVertex[this.vertices.Length];

            for (int i = 0; i < this.vertices.Length; i++)
            {
                vertices[i] = new WallVertex(new Vector3(this.vertices[i] + offset), Vector3.UnitZ);
            }

            var id = surface.AddVertices(vertices);

            var indices = new ushort[this.triangles.Length * 3];

            int j = 0;
            for (int i = 0; i < this.triangles.Length; i++)
            {
                var triangle = this.triangles[i];
                indices[j++] = (ushort)(triangle.Id0 + id);
                indices[j++] = (ushort)(triangle.Id1 + id);
                indices[j++] = (ushort)(triangle.Id2 + id);
            }

            surface.AddIndices(indices);
        }
    }
}
