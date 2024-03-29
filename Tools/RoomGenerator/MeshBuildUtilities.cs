#if DREAMTECK_SPLINES

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Larje.Core.Tools.RoomGenerator
{
    public static class MeshBuildUtilities
    {
        public static void BuildWall(WallBuildData data)
        {
            SortedDictionary<float, bool> xPositions = new SortedDictionary<float, bool>();
            xPositions.Add(0f, true);
            xPositions.Add(1f, true);
            
            SortedDictionary<float, bool> yPositions = new SortedDictionary<float, bool>();
            yPositions.Add(0f, true);
            yPositions.Add(1f, true);

            if (data.holes != null)
            {
                data.holes.ForEach(hole =>
                {
                    GetHoleBounds(data, hole, out Vector2 min, out Vector2 max);

                    if (IsPairInBounds(min.x, max.x))
                    {
                        if (IsPairInBounds(min.y, max.y))
                        {
                            TryAddValue(yPositions, min.y, false);
                            TryAddValue(yPositions, max.y, true);
                        }
                        
                        TryAddValue(xPositions, min.x, false);
                        TryAddValue(xPositions, max.x, true);
                    }
                });
            }

            for (int y = 0; y < yPositions.Count - 1; y++)
            {
                for (int x = 0; x < xPositions.Count - 1; x++)
                {
                    if (xPositions.Values.ToArray()[x] || yPositions.Values.ToArray()[y])
                    {
                        Vector3 upOffset = Vector3.up * data.height * yPositions.Keys.ToArray()[y];

                        WallBuildData partData = data;

                        partData.usePrev = data.usePrev && x == 0;
                        partData.useNext = data.useNext && x == xPositions.Count - 2;

                        partData.from = Vector3.Lerp(data.from, data.to, xPositions.Keys.ToArray()[x]) + upOffset;
                        partData.to = Vector3.Lerp(data.from, data.to, xPositions.Keys.ToArray()[x + 1]) + upOffset;

                        partData.widthBottom = Mathf.Lerp(data.widthBottom, data.widthTop, yPositions.Keys.ToArray()[y]);
                        partData.widthTop = Mathf.Lerp(data.widthBottom, data.widthTop, yPositions.Keys.ToArray()[y + 1]);

                        partData.prev += upOffset;
                        partData.next += upOffset;

                        partData.height = data.height * (yPositions.Keys.ToArray()[y + 1] - yPositions.Keys.ToArray()[y]);

                        List<Vector3> points = GetBoxPoints(partData);
                        data.vertOffset = BuildBox(points, data.vertOffset, !partData.usePrev, 
                            !partData.useNext, partData.buildTop, partData.buildBottom, data.verts, data.tris,
                            data.vertexColors, data.topColor, data.bottomColor);
                    }
                }
            }
        }

        private static bool IsPairInBounds(float a, float b)
        {
            bool result = (a is >= 0f and <= 1f) || (b is >= 0f and <= 1f);
            result |= (a < 0f && b > 1);
            return result;
        }

        private static void TryAddValue(SortedDictionary<float, bool> dic, float key, bool value)
        {
            key = Mathf.Clamp01(key);
            if (key is >= 0f and <= 1f)
            {
                if (!dic.ContainsKey(key))
                {
                    dic.Add(key, value);   
                }
                else
                {
                    dic[key] &= value;
                }
            }
        }

        private static List<Vector3> GetBoxPoints(WallBuildData data)
        {
            Vector3 perp = (data.to - data.from).normalized;
            perp = new Vector3(perp.z, 0f, -perp.x);

            List<Vector3> points = new List<Vector3>();
            points.Add(data.from + perp * data.widthBottom);
            points.Add(data.from - perp * data.widthBottom);
            points.Add(data.to + perp * data.widthBottom);
            points.Add(data.to - perp * data.widthBottom);

            if (data.usePrev)
            {
                Vector3 prevPerp = (data.from - data.prev).normalized;
                prevPerp = new Vector3(prevPerp.z, 0f, -prevPerp.x);

                Vector3 prevPointA = data.prev + prevPerp * data.widthBottom;
                Vector3 toPointA = data.to + perp * data.widthBottom;
                if (LineLineIntersection(out Vector3 interA, prevPointA, data.from - data.prev, toPointA,
                        data.from - data.to))
                {
                    points[0] = interA;
                }

                Vector3 prevPointB = data.prev - prevPerp * data.widthBottom;
                Vector3 toPointB = data.to - perp * data.widthBottom;
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

                Vector3 fromPointC = data.from + perp * data.widthBottom;
                Vector3 nextPointC = data.next + nextPerp * data.widthBottom;
                if (LineLineIntersection(out Vector3 interC, fromPointC, data.to - data.from, nextPointC,
                        data.to - data.next))
                {
                    points[2] = interC;
                }

                Vector3 fromPointD = data.from - perp * data.widthBottom;
                Vector3 nextPointD = data.next - nextPerp * data.widthBottom;
                if (LineLineIntersection(out Vector3 interD, fromPointD, data.to - data.from, nextPointD,
                        data.to - data.next))
                {
                    points[3] = interD;
                }
            }

            float widthModifier = data.widthTop / data.widthBottom;
            points.Add((data.from + (points[0] - data.from) * widthModifier) + (Vector3.up * data.height));
            points.Add((data.from + (points[1] - data.from) * widthModifier) + (Vector3.up * data.height));
            points.Add((data.to + (points[2] - data.to) * widthModifier) + (Vector3.up * data.height));
            points.Add((data.to + (points[3] - data.to) * widthModifier) + (Vector3.up * data.height));

            return points;
        }

        private static int BuildBox(List<Vector3> points, int offset, bool buildFromSide, bool buildToSide, 
            bool buildTop, bool buildBottom, List<Vector3> verts, List<int> tris, 
            List<Color> colors, Color tColor, Color bColor)
        {
            int newOffset = offset;

            if (buildFromSide)
            {
                BuildPlane(points[1], points[5], points[4], points[0], newOffset, verts, tris);
                colors.Add(bColor); colors.Add(tColor); colors.Add(tColor); colors.Add(bColor);
                newOffset += 4;
            }

            if (buildToSide)
            {
                BuildPlane(points[2], points[6], points[7], points[3], newOffset, verts, tris);
                colors.Add(bColor); colors.Add(tColor); colors.Add(tColor); colors.Add(bColor);
                newOffset += 4;
            }

            BuildPlane(points[0], points[4], points[6], points[2], newOffset, verts, tris);
            colors.Add(bColor); colors.Add(tColor); colors.Add(tColor); colors.Add(bColor);
            newOffset += 4;
            
            BuildPlane(points[3], points[7], points[5], points[1], newOffset, verts, tris);
            colors.Add(bColor); colors.Add(tColor); colors.Add(tColor); colors.Add(bColor);
            newOffset += 4;

            if (buildTop)
            {
                BuildPlane(points[4], points[5], points[7], points[6], newOffset, verts, tris);
                colors.Add(tColor); colors.Add(tColor); colors.Add(tColor); colors.Add(tColor);
                newOffset += 4;
            }

            if (buildBottom)
            {
                BuildPlane(points[0], points[2], points[3], points[1], newOffset, verts, tris);
                colors.Add(bColor); colors.Add(bColor); colors.Add(bColor); colors.Add(bColor);
            }

            newOffset += 4;

            return newOffset;
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

        private static void GetHoleBounds(WallBuildData data, SplineWallHole.Data hole, out Vector2 min, out Vector2 max)
        {
            min = Vector2.zero;
            max = Vector2.zero;

            float length = data.toDistance - data.fromDistance;
            
            min.x = ((hole.distance - hole.size.x / 2f) - data.fromDistance) / length;
            max.x = ((hole.distance + hole.size.x / 2f) - data.fromDistance) / length;

            float yPosPercent = hole.yPos / data.height;
            float ySizePercent = hole.size.y / data.height;
            
            min.y = yPosPercent;
            max.y = yPosPercent + ySizePercent;
        }

        public struct WallBuildData
        {
            public bool isClosed;
            
            public Vector3 prev;
            public Vector3 from;
            public Vector3 to;
            public Vector3 next;

            public float fromDistance;
            public float toDistance;
            public float fullDistance;

            public bool usePrev;
            public bool useNext;

            public bool buildTop;
            public bool buildBottom;

            public float widthTop;
            public float widthBottom;

            public float height;
            public int vertOffset;
            
            public List<Vector3> verts;
            public List<int> tris;
            public List<Color> vertexColors;
            public List<SplineWallHole.Data> holes;

            public Color topColor;
            public Color bottomColor;
        }
    }
}
#endif