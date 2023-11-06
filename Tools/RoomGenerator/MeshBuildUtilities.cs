#if DREAMTECK_SPLINES

using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools.RoomGenerator
{
    public static class MeshBuildUtilities
    {
        public static void BuildWall(WallBuildData data)
        {
            Vector3 perp = (data.to - data.from).normalized;
            perp = new Vector3(perp.z, 0f, -perp.x);

            Vector3 a = data.from + perp * data.width;
            Vector3 b = data.from - perp * data.width;
            Vector3 c = data.to + perp * data.width;
            Vector3 d = data.to - perp * data.width;

            if (data.usePrev)
            {
                Vector3 prevPerp = (data.from - data.prev).normalized;
                prevPerp = new Vector3(prevPerp.z, 0f, -prevPerp.x);

                Vector3 prevPointA = data.prev + prevPerp * data.width;
                Vector3 toPointA = data.to + perp * data.width;
                if (LineLineIntersection(out Vector3 interA, prevPointA, data.from - data.prev, toPointA,
                        data.from - data.to))
                {
                    a = interA;
                }

                Vector3 prevPointB = data.prev - prevPerp * data.width;
                Vector3 toPointB = data.to - perp * data.width;
                if (LineLineIntersection(out Vector3 interB, prevPointB, data.from - data.prev, toPointB,
                        data.from - data.to))
                {
                    b = interB;
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
                    c = interC;
                }

                Vector3 fromPointD = data.from - perp * data.width;
                Vector3 nextPointD = data.next - nextPerp * data.width;
                if (LineLineIntersection(out Vector3 interD, fromPointD, data.to - data.from, nextPointD,
                        data.to - data.next))
                {
                    d = interD;
                }
            }

            Vector3 aUp = a + Vector3.up * data.height;
            Vector3 bUp = b + Vector3.up * data.height;
            Vector3 cUp = c + Vector3.up * data.height;
            Vector3 dUp = d + Vector3.up * data.height;

            BuildPlane(b, bUp, aUp, a, data.vertOffset, data.verts, data.tris);
            BuildPlane(c, cUp, dUp, d, data.vertOffset + 4, data.verts, data.tris);
            BuildPlane(a, aUp, cUp, c, data.vertOffset + 8, data.verts, data.tris);
            BuildPlane(d, dUp, bUp, b, data.vertOffset + 12, data.verts, data.tris);
            BuildPlane(aUp, bUp, dUp, cUp, data.vertOffset + 16, data.verts, data.tris);
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

        public class WallBuildData
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