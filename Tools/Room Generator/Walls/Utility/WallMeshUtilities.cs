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
            Vector3 offsetFrom = GetOffsetFrom(wall, segment);
            Vector3 offsetTo = GetOffsetTo(wall, segment);

            List<Vector3> topPoints = new List<Vector3>()
            {
                segment.Corners[1] + offsetFrom,
                segment.Corners[1] - offsetFrom,
                segment.Corners[2] - offsetTo,
                segment.Corners[2] + offsetTo
            };
            BuildPlane(topPoints.ToArray(), properties, true);

            List<Vector3> bottomPoints = new List<Vector3>()
            {
                segment.Corners[0] + offsetFrom,
                segment.Corners[0] - offsetFrom,
                segment.Corners[3] - offsetTo,
                segment.Corners[3] + offsetTo
            };
            BuildPlane(bottomPoints.ToArray(), properties, false);
        }
        
        private static void BuildTopBottomPlanes(SplineWall wall, WallSegment segment, MeshProperties properties)
        {
            /*Vector3 offset = segment.Right * wall.Config.Width * 0.5f;

            List<Vector3> topPoints = new List<Vector3>()
            {
                segment.Corners[1] + offset,
                segment.Corners[1] - offset,
                segment.Corners[2] - offset,
                segment.Corners[2] + offset
            };
            BuildPlane(topPoints.ToArray(), properties, true);
            
            List<Vector3> bottomPoints = new List<Vector3>()
            {
                segment.Corners[0] + offset,
                segment.Corners[0] - offset,
                segment.Corners[3] - offset,
                segment.Corners[3] + offset
            };
            BuildPlane(bottomPoints.ToArray(), properties, false);*/
        }
        
        private static void BuildForwardBackPlanes(SplineWall wall, WallSegment segment, MeshProperties properties)
        {
            /*Vector3 offset = segment.Right * wall.Config.Width * 0.5f;

            List<Vector3> topPoints = new List<Vector3>()
            {
                segment.Corners[0] + offset,
                segment.Corners[0] - offset,
                segment.Corners[1] - offset,
                segment.Corners[1] + offset
            };
            BuildPlane(topPoints.ToArray(), properties, true);
            
            List<Vector3> bottomPoints = new List<Vector3>()
            {
                segment.Corners[3] + offset,
                segment.Corners[3] - offset,
                segment.Corners[2] - offset,
                segment.Corners[2] + offset
            };
            BuildPlane(bottomPoints.ToArray(), properties, false);*/
        }

        private static void BuildPlane(Vector3[] points, MeshProperties properties, bool normalsOutside = true)
        {
            Vector3[] vertices = points;
            int[] triangles = normalsOutside ? TrianglesOutside : TrianglesInside;
            
            int vertexIndex = properties.Vertices.Count;
            properties.Vertices.AddRange(vertices);
            properties.Triangles.AddRange(triangles.Select(i => i + vertexIndex));
        }

        private static Vector3 GetOffsetFrom(SplineWall wall, WallSegment segment)
        {
            float width = wall.Config.Width * 0.5f;
            if (segment.Prev is { Hidden: false })
            {
                Vector3 thisCenter = Vector3.Lerp(segment.Corners[0], segment.Corners[3], 0.5f) + segment.Right * width;
                Vector3 prevCenter = Vector3.Lerp(segment.Prev.Corners[0], segment.Prev.Corners[3], 0.5f) + segment.Prev.Right * width;
                thisCenter.MMSetY(0);
                prevCenter.MMSetY(0);
                
                if (GetIntersection(out Vector3 inter, thisCenter, -segment.Forward, prevCenter, segment.Prev.Forward))
                {
                    return inter - segment.Corners[0];
                }
            }
            
            return segment.Right * width;
        }
        
        private static Vector3 GetOffsetTo(SplineWall wall, WallSegment segment)
        {
            float width = wall.Config.Width * 0.5f;
            if (segment.Next is { Hidden: false })
            {
                Vector3 thisCenter = Vector3.Lerp(segment.Corners[0], segment.Corners[3], 0.5f) + segment.Right * width;
                Vector3 nextCenter = Vector3.Lerp(segment.Next.Corners[0], segment.Next.Corners[3], 0.5f) + segment.Next.Right * width;
                thisCenter.MMSetY(0);
                nextCenter.MMSetY(0);
                if (GetIntersection(out Vector3 inter, thisCenter, segment.Forward, nextCenter, -segment.Next.Forward))
                {
                    return inter - segment.Corners[3];
                }
            }
            
            return segment.Right * width;
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
        
        private static bool GetIntersection(out Vector3 intersection, Vector3 pointA, Vector3 dirA, Vector3 pointB, Vector3 dirB)
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