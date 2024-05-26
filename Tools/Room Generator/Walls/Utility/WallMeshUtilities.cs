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
            WallSegmentUtilities.GetSegments(wall)
                .ForEach(segment => BuildSegmentMesh(wall, segment, properties));   
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
            List<Vector3> topPoints = new List<Vector3>()
            {
                segment.Corners[0] + segment.OffsetFrom,
                segment.Corners[1] + segment.OffsetFrom,
                segment.Corners[2] + segment.OffsetTo,
                segment.Corners[3] + segment.OffsetTo
            };
            BuildPlane(topPoints.ToArray(), properties, true);

            List<Vector3> bottomPoints = new List<Vector3>()
            {
                segment.Corners[0] - segment.OffsetFrom,
                segment.Corners[1] - segment.OffsetFrom,
                segment.Corners[2] - segment.OffsetTo,
                segment.Corners[3] - segment.OffsetTo
            };
            BuildPlane(bottomPoints.ToArray(), properties, false);
        }
        
        private static void BuildTopBottomPlanes(SplineWall wall, WallSegment segment, MeshProperties properties)
        {
            List<Vector3> topPoints = new List<Vector3>()
            {
                segment.Corners[1] + segment.OffsetFrom,
                segment.Corners[1] - segment.OffsetFrom,
                segment.Corners[2] - segment.OffsetTo,
                segment.Corners[2] + segment.OffsetTo
            };
            BuildPlane(topPoints.ToArray(), properties, true);
            
            List<Vector3> bottomPoints = new List<Vector3>()
            {
                segment.Corners[0] + segment.OffsetFrom,
                segment.Corners[0] - segment.OffsetFrom,
                segment.Corners[3] - segment.OffsetTo,
                segment.Corners[3] + segment.OffsetTo
            };
            BuildPlane(bottomPoints.ToArray(), properties, false);
        }
        
        private static void BuildForwardBackPlanes(SplineWall wall, WallSegment segment, MeshProperties properties)
        {
            List<Vector3> topPoints = new List<Vector3>()
            {
                segment.Corners[0] + segment.OffsetFrom,
                segment.Corners[0] - segment.OffsetFrom,
                segment.Corners[1] - segment.OffsetFrom,
                segment.Corners[1] + segment.OffsetFrom
            };
            BuildPlane(topPoints.ToArray(), properties, true);
            
            List<Vector3> bottomPoints = new List<Vector3>()
            {
                segment.Corners[3] + segment.OffsetTo,
                segment.Corners[3] - segment.OffsetTo,
                segment.Corners[2] - segment.OffsetTo,
                segment.Corners[2] + segment.OffsetTo
            };
            BuildPlane(bottomPoints.ToArray(), properties, false);
        }

        private static void BuildPlane(Vector3[] points, MeshProperties properties, bool normalsOutside = true)
        {
            Vector3[] vertices = points;
            int[] triangles = normalsOutside ? TrianglesOutside : TrianglesInside;
            
            int vertexIndex = properties.Vertices.Count;
            properties.Vertices.AddRange(vertices);
            properties.Triangles.AddRange(triangles.Select(i => i + vertexIndex));
        }
        
        private static void ApplyMesh(Mesh mesh, MeshProperties properties)
        {
            mesh.Clear();
            mesh.vertices = properties.Vertices.ToArray();
            mesh.triangles = properties.Triangles.ToArray();
            mesh.colors = properties.Colors.ToArray();

            //mesh.subMeshCount = properties.Submeshes.Count;
            //mesh.SetSubMeshes(properties.Submeshes.ToArray());

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
        }

        private class MeshProperties
        {
            public List<Vector3> Vertices = new List<Vector3>(); 
            public List<int> Triangles = new List<int>();
            public List<Color> Colors = new List<Color>();
            public List<SubMeshDescriptor> Submeshes = new List<SubMeshDescriptor>();
        }
    }
}
#endif