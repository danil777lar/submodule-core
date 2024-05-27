#if DREAMTECK_SPLINES

using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Rendering;

namespace Larje.Core.Tools.RoomGenerator
{
    public static class WallMeshUtilities
    {
        private static int[] TrianglesOutside => new []{0, 1, 2, 0, 2, 3}; 
        private static int[] TrianglesInside => new []{2, 1, 0, 3, 2, 0}; 
        
        public static void BuildMesh(SplineWall wall, Mesh mesh)
        {
            MeshProperties properties = new MeshProperties();

            int wallRow = -1;
            SubMeshDescriptor submesh = new SubMeshDescriptor();
                
            foreach (WallSegment segment in WallSegmentUtilities.GetSegments(wall).OrderBy(x => x.RowIndex))
            {
                if (wallRow != segment.RowIndex)
                {
                    if (wallRow != -1)
                    {
                        submesh.indexCount = properties.Triangles.Count - submesh.indexStart;
                        properties.Submeshes.Add(submesh);
                    }
                    
                    wallRow = segment.RowIndex;
                    
                    submesh = new SubMeshDescriptor();
                    submesh.topology = MeshTopology.Triangles;
                    submesh.indexStart = properties.Triangles.Count;
                }
                
                BuildSegmentMesh(wall, segment, properties);
            }
            
            submesh.indexCount = properties.Triangles.Count - submesh.indexStart;
            properties.Submeshes.Add(submesh);
            
            ApplyMesh(mesh, properties);
        }

        private static void BuildSegmentMesh(SplineWall wall, WallSegment segment, MeshProperties properties)
        {
            if (!segment.Hidden)
            {
                BuildRightLeftPlanes(wall, segment, properties);
                BuildTopBottomPlanes(wall, segment, properties);
                BuildForwardBackPlanes(wall, segment, properties);
            }
        }

        private static void BuildRightLeftPlanes(SplineWall wall, WallSegment segment, MeshProperties properties)
        {
            List<IndexDirection> right = new()
            {
                new IndexDirection(0, true), new IndexDirection(1, true),
                new IndexDirection(2, true), new IndexDirection(3, true)
            };
            BuildPlane(segment, right, properties, true);

            List<IndexDirection> left = new()
            {
                new IndexDirection(0, false), new IndexDirection(1, false), 
                new IndexDirection(2, false), new IndexDirection(3, false)
            };
            BuildPlane(segment, left, properties, false);
        }
        
        private static void BuildTopBottomPlanes(SplineWall wall, WallSegment segment, MeshProperties properties)
        {
            List<IndexDirection> top = new() { 
                new IndexDirection(1, true), new IndexDirection(1, false), 
                new IndexDirection(2, false), new IndexDirection(2, true)};
            BuildPlane(segment, top, properties, true);
            
            List<IndexDirection> bottom = new() { 
                new IndexDirection(0, true), new IndexDirection(0, false), 
                new IndexDirection(3, false), new IndexDirection(3, true)};
            BuildPlane(segment, bottom, properties, false);
        }
        
        private static void BuildForwardBackPlanes(SplineWall wall, WallSegment segment, MeshProperties properties)
        {
            if (segment.Next == null || segment.Next.Hidden)
            {
                List<IndexDirection> forward = new()
                {
                    new IndexDirection(3, true), new IndexDirection(3, false),
                    new IndexDirection(2, false), new IndexDirection(2, true)
                };
                BuildPlane(segment, forward, properties, false);
            }

            if (segment.Prev == null || segment.Prev.Hidden)
            {
                List<IndexDirection> back = new()
                {
                    new IndexDirection(0, true), new IndexDirection(0, false),
                    new IndexDirection(1, false), new IndexDirection(1, true)
                };
                BuildPlane(segment, back, properties, true);
            }
        }

        private static void BuildPlane(WallSegment segment, List<IndexDirection> indexes, MeshProperties properties, bool normalsOutside = true)
        {
            Vector3[] vertices = indexes.Select(i => CornerToPoint(segment, i.index, i.right)).ToArray();
            int[] triangles = normalsOutside ? TrianglesOutside : TrianglesInside;
            List<Color> colors = indexes.Select(i => IndexToColor(segment, i.index)).ToList();
            
            int vertexIndex = properties.Vertices.Count;
            properties.Vertices.AddRange(vertices);
            properties.Triangles.AddRange(triangles.Select(i => i + vertexIndex));
            properties.Colors.AddRange(colors);
        }

        private static Color IndexToColor(WallSegment segment, int index)
        {
            return index == 0 || index == 3 ? segment.VertexColorBottom : segment.VertexColorTop;
        }
        
        private static void ApplyMesh(Mesh mesh, MeshProperties properties)
        {
            mesh.Clear();
            mesh.vertices = properties.Vertices.ToArray();
            mesh.triangles = properties.Triangles.ToArray();
            mesh.colors = properties.Colors.ToArray();

            mesh.subMeshCount = properties.Submeshes.Count;
            mesh.SetSubMeshes(properties.Submeshes.ToArray());

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
        }

        private static Vector3 CornerToPoint(WallSegment segment, int corner, bool right)
        {
            Vector3 offset = (corner == 0 || corner == 1) ? segment.OffsetFrom : segment.OffsetTo;
            float multiplier =  (corner == 0 || corner == 3) ? segment.WidthMultiplierBottom : segment.WidthMultiplierTop;
            float direction = right ? 1 : -1;

            return segment.Corners[corner] + offset * multiplier * direction;
        }

        private class MeshProperties
        {
            public List<Vector3> Vertices = new List<Vector3>(); 
            public List<int> Triangles = new List<int>();
            public List<Color> Colors = new List<Color>();
            public List<SubMeshDescriptor> Submeshes = new List<SubMeshDescriptor>();
        }

        private struct IndexDirection
        {
            public int index;
            public bool right;
            
            public IndexDirection(int index, bool right)
            {
                this.index = index;
                this.right = right;
            }
        }
    }
}
#endif