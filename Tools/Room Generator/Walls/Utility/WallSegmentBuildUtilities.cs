#if DREAMTECK_SPLINES

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace Larje.Core.Tools.RoomGenerator
{
    public static class WallSegmentBuildUtilities
    {
        public static void BuildWallSegmentsMesh(Mesh mesh)
        {
            
            
            //ApplyMesh();
        }
        
        private static void ApplyMesh(Mesh mesh, List<Vector3> verts, List<int> tris, List<Color> colors, List<SubMeshDescriptor> submeshes)
        {
            mesh.Clear();
            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.colors = colors.ToArray();

            mesh.subMeshCount = 2;
            mesh.SetSubMeshes(submeshes);

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
    }
}
#endif