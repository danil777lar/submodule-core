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
                .ForEach(segment => BuildSegmentMesh(segment, properties));   
            ApplyMesh(mesh, properties);
        }

        private static void BuildSegmentMesh(WallSegment segment, MeshProperties properties)
        {
            if (!segment.Hidden)
            {
                BuildRightLeftPlanes(segment, properties);
                BuildTopBottomPlanes(segment, properties);
                BuildForwardBackPlanes(segment, properties);
            }
        }

        private static void BuildRightLeftPlanes(WallSegment segment, MeshProperties properties)
        {
            BuildPlane(segment.Corners, properties, true);
            BuildPlane(segment.Corners, properties, false);
        }
        
        private static void BuildTopBottomPlanes(WallSegment segment, MeshProperties properties)
        {
            
        }
        
        private static void BuildForwardBackPlanes(WallSegment segment, MeshProperties properties)
        {
            
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
        
        private static bool LineLineIntersection(out Vector3 intersection, Vector3 pointA, Vector3 dirA, Vector3 pointB, Vector3 dirB)
        {
            Vector3 crossVec1and2 = Vector3.Cross(dirA, dirB);
            float planarFactor = Vector3.Dot(pointB - pointA, crossVec1and2);

            if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
            {
                float s = Vector3.Dot(Vector3.Cross(pointB - pointA, dirB), crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = pointA + (dirA * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
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