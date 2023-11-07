#if DREAMTECK_SPLINES

using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools.RoomGenerator
{
    public static class MeshBuildUtilities
    {
        public static void BuildWall(WallBuildData data)
        {
            List<float> xPositions = new List<float>();
            xPositions.Add(0f);
            xPositions.Add(1f);
            
            List<float> yPositions = new List<float>();
            yPositions.Add(0f);
            yPositions.Add(1f);

            if (data.holes != null)
            {
                data.holes.ForEach(hole =>
                {
                    
                });   
            }

            for (int y = 0; y < yPositions.Count - 1; y++)
            {
                for (int x = 0; x < xPositions.Count - 1; x++)
                {
                    Vector3 upOffset = Vector3.up * data.height * yPositions[y];
                    
                    WallBuildData partData = data;
                    partData.usePrev = data.usePrev && x == 0;
                    partData.useNext = data.useNext && x == xPositions.Count - 2;
                    partData.prev += upOffset; 
                    partData.from = Vector3.Lerp(data.from, data.to, xPositions[x]) + upOffset;
                    partData.to = Vector3.Lerp(data.from, data.to, xPositions[x + 1]) + upOffset;
                    partData.next += upOffset;
                    partData.height = data.height * (yPositions[y + 1] - yPositions[y]); 
                    
                    List<Vector3> points = GetBoxPoints(partData);
                    data.vertOffset = BuildBox(points, data.vertOffset, data.verts, data.tris);       
                }
            }
        }

        private static List<Vector3> GetBoxPoints(WallBuildData data)
        {
            Vector3 perp = (data.to - data.from).normalized;
            perp = new Vector3(perp.z, 0f, -perp.x);

            List<Vector3> points = new List<Vector3>();
            points.Add(data.from + perp * data.width);
            points.Add(data.from - perp * data.width);
            points.Add(data.to + perp * data.width);
            points.Add(data.to - perp * data.width);

            if (data.usePrev)
            {
                Vector3 prevPerp = (data.from - data.prev).normalized;
                prevPerp = new Vector3(prevPerp.z, 0f, -prevPerp.x);

                Vector3 prevPointA = data.prev + prevPerp * data.width;
                Vector3 toPointA = data.to + perp * data.width;
                if (LineLineIntersection(out Vector3 interA, prevPointA, data.from - data.prev, toPointA,
                        data.from - data.to))
                {
                    points[0] = interA;
                }

                Vector3 prevPointB = data.prev - prevPerp * data.width;
                Vector3 toPointB = data.to - perp * data.width;
                if (LineLineIntersection(out Vector3 interB, prevPointB, data.from - data.prev, toPointB,
                        data.from - data.to))
                {
                    points[1] = interB;
                }
            }

            if (data.useNext)
            {
                Vector3 nextPerp = (data.next - data.to).normalized;
                nextPerp = new Vector3(nextPerp.z, 0f, -nextPerp.x);

                Vector3 fromPointC = data.from + perp * data.width;
                Vector3 nextPointC = data.next + nextPerp * data.width;
                if (LineLineIntersection(out Vector3 interC, fromPointC, data.to - data.from, nextPointC,
                        data.to - data.next))
                {
                    points[2] = interC;
                }

                Vector3 fromPointD = data.from - perp * data.width;
                Vector3 nextPointD = data.next - nextPerp * data.width;
                if (LineLineIntersection(out Vector3 interD, fromPointD, data.to - data.from, nextPointD,
                        data.to - data.next))
                {
                    points[3] = interD;
                }
            }

            points.Add(points[0] + Vector3.up * data.height);
            points.Add(points[1] + Vector3.up * data.height);
            points.Add(points[2] + Vector3.up * data.height);
            points.Add(points[3] + Vector3.up * data.height);

            return points;
        }

        private static int BuildBox(List<Vector3> points, int offset, List<Vector3> verts, List<int> tris)
        {
            BuildPlane(points[1], points[5], points[4], points[0], offset, verts, tris);
            BuildPlane(points[2], points[6], points[7], points[3], offset + 4, verts, tris);
            BuildPlane(points[0], points[4], points[6], points[2], offset + 8, verts, tris);
            BuildPlane(points[3], points[7], points[5], points[1], offset + 12, verts, tris);
            BuildPlane(points[4], points[5], points[7], points[6], offset + 16, verts, tris);

            return offset + 20;
        }

        private static void BuildPlane(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int vertOffset, List<Vector3> verts, List<int> tris)
        {
            verts.AddRange(new Vector3[] { a, b, c, d });
            tris.AddRange(new int[] { 0 + vertOffset, 1 + vertOffset, 2 + vertOffset });
            tris.AddRange(new int[] { 0 + vertOffset, 2 + vertOffset, 3 + vertOffset });
        }

        private static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1,
            Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {
            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //is coplanar, and not parallel
            if (Mathf.Abs(planarFactor) < 0.0001f
                && crossVec1and2.sqrMagnitude > 0.0001f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2)
                          / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + (lineVec1 * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }

        public struct WallBuildData
        {
            public Vector3 prev;
            public Vector3 from;
            public Vector3 to;
            public Vector3 next;

            public bool usePrev;
            public bool useNext;

            public float width;
            public float height;
            public int vertOffset;
            public List<Vector3> verts;
            public List<int> tris;
            
            public List<SplineWallHole.Data> holes;
        }
    }
}
#endif