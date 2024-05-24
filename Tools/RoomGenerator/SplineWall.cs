#if DREAMTECK_SPLINES

using System;
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using JetBrains.Annotations;
using Larje.Core.Services;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace Larje.Core.Tools.RoomGenerator
{
    [RequireComponent(typeof(SplineComputer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class SplineWall : MonoBehaviour
    {
        [SerializeField] private SplineWallConfig config;
        [SerializeField, Min(0f)] private float segmentsPerUnit = 0f;
        [SerializeField] private bool rebuildOnStart = false;
        [SerializeField] private List<bool> hideWallParts = new List<bool>();

        private SplineComputer _spline;

        public SplineComputer SplineInstance
        {
            get
            {
                if (_spline == null)
                {
                    _spline = GetComponent<SplineComputer>();
                }

                return _spline;
            }
        }

        public void Initialize()
        {
            Rebuild();
        }

        public void Initialize(Material wallBottomMaterial, Material wallTopMaterial)
        {
            Rebuild();
            MeshRenderer rend = GetComponent<MeshRenderer>();
            rend.materials = new Material[] { wallBottomMaterial, wallTopMaterial };
        }

        private void Start()
        {
            if (rebuildOnStart)
            {
                Rebuild();
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                Rebuild();
            }
            
            foreach (WallSegment segment in GetSegments())
            {
                Gizmos.color = Color.red;
                
                Gizmos.DrawSphere(segment.Min, 0.1f);
                Vector3 center = (segment.Min + (segment.Max - segment.Min) * 0.5f) + Vector3.up;
                
                Gizmos.color = segment.Hidden ? Color.blue.SetAlpha(1f) : Color.red.SetAlpha(0.5f);
                Gizmos.DrawLine(segment.Min,  center);
                Gizmos.DrawLine(center,  segment.Max);
            }
        }

        private void OnValidate()
        {
            if (SplineInstance != null)
            {
                while (hideWallParts.Count < SplineInstance.GetPoints().Length)
                {
                    hideWallParts.Add(false);
                }

                while (hideWallParts.Count > SplineInstance.GetPoints().Length)
                {
                    hideWallParts.RemoveAt(hideWallParts.Count - 1);
                }
            }
        }
        
        private void Rebuild()
        {
            
        }

        /*private void Rebuild()
        {
            Mesh mesh = GetMesh();

            if (SplineInstance == null || config == null || config.WallParts.Length == 0)
            {
                return;
            }

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Color> vertexColors = new List<Color>();
            List<Vector3> points = GetPoints(out List<int> hiddenSegments);
            List<SubMeshDescriptor> submeshes = new List<SubMeshDescriptor>();
            List<Material> materials = new List<Material>();
            List<SplineWallHole.Data> holes = GetHoles();
            
            float lastHeights = 0f;
            foreach (SplineWallConfig.WallPart wallPart in config.WallParts)
            {
                float partHeightPercent = wallPart.Weight / config.GetWeightsSum();
                float partOffset = lastHeights;
                float partHeight = config.Height * partHeightPercent;
                float partWidthTop = config.Width * wallPart.TopWidthMultiplier;
                float partWidthBottom = config.Width * wallPart.BottomWidthMultiplier;
                lastHeights += partHeight;
                
                materials.Add(wallPart.Material);
                
                SubMeshDescriptor submesh = new SubMeshDescriptor();
                submesh.indexStart = triangles.Count;
                BuildWall(
                    points, 
                    vertices, 
                    triangles, 
                    vertexColors, 
                    partOffset, 
                    partHeight, 
                    partWidthTop,
                    partWidthBottom, 
                    wallPart.DrawTop,
                    wallPart.DrawBottom,
                    wallPart.VertexColorTop, 
                    wallPart.VertexColorBottom,  
                    holes, 
                    hiddenSegments);
                submesh.indexCount = triangles.Count - submesh.indexStart;
                submeshes.Add(submesh);
            }

            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = vertexColors.ToArray();

            mesh.subMeshCount = 2;
            mesh.SetSubMeshes(submeshes);

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            MeshRenderer rend = GetComponent<MeshRenderer>();
            rend.sharedMaterials = materials.ToArray();

            if (mesh.vertices.Length > 0)
            {
                MeshCollider collider = GetComponent<MeshCollider>();
                collider.sharedMesh = mesh;
            }
        }*/

        private List<WallSegment> GetSegments()
        {
            List<WallSegment> segments = new List<WallSegment>();
            List<double> percents = GetPointPercents();
            for (int i = 0; i < percents.Count - 1; i++)
            {
                Vector3 min = SplineInstance.EvaluatePosition(percents[i]);
                Vector3 max = SplineInstance.EvaluatePosition(percents[i + 1]);
                
                WallSegment segment = new WallSegment(min, max);
                
                segments.Add(segment);
            }
            
            MarkHiddenSegments(segments);
            
            return segments;
        }
        
        private void MarkHiddenSegments(IReadOnlyCollection<WallSegment> segments)
        {
            foreach (PercentInterval interval in GetHiddenIntervals())
            {
                foreach (WallSegment segment in segments)
                {
                    segment.Hidden |= interval.Contains(GetSegmentCenterPercent(segment));
                }
            }
        }

        private List<PercentInterval> GetHiddenIntervals()
        {
            List<PercentInterval> intervals = new List<PercentInterval>();
            GetComponentsInChildren<SplineWallHole>().ToList().ForEach(x => 
                intervals.Add(new PercentInterval(x.GetData().XFrom, x.GetData().XTo)));

            return intervals;
        }

        private List<double> GetPointPercents()
        {
            List<double> percents = new List<double>();
            GetPoints().ForEach(x => percents.Add(SplineInstance.Project(x).percent));
            
            foreach (SplineWallHole hole in GetComponentsInChildren<SplineWallHole>())
            {
                SplineWallHole.Data holeData = hole.GetData();
                percents.Add(holeData.XFrom);
                percents.Add(holeData.XTo);
            }

            percents = percents.OrderBy(x => x).ToList();
            
            percents.Add(1);
            
            return percents;
        }
        
        private List<Vector3> GetPoints()
        {
            List<Vector3> points = new List<Vector3>();
            List<SplinePoint> splinePoints = SplineInstance.GetPoints().ToList();

            if (SplineInstance.isClosed)
            {
                splinePoints.Add(splinePoints[0]);
            }
            
            for (int i = 0; i < splinePoints.Count - 1; i++)
            {
                points.Add(splinePoints[i].position);
                float lengthToNext = SplineInstance.CalculateLength(SplineInstance.GetPointPercent(i),
                    SplineInstance.GetPointPercent(i + 1));
                int pointsCount = Mathf.RoundToInt(lengthToNext * segmentsPerUnit);
                for (int j = 1; j < pointsCount; j++)
                {
                    float percent = Mathf.Lerp(
                        (float)SplineInstance.GetPointPercent(i),
                        (float)SplineInstance.GetPointPercent(i + 1),
                        (float)j / (float)pointsCount);

                    points.Add(SplineInstance.EvaluatePosition(percent));
                }
            }
            
            return points;
        }

        private double GetSegmentCenterPercent(WallSegment segment)
        {
            float distance = Vector3.Distance(segment.Min.XZ(), segment.Max.XZ());
            double fromPercent = SplineInstance.Project(segment.Min).percent;
            return SplineInstance.Travel(fromPercent, distance * 0.5f);
        }

        private void BuildWall(List<Vector3> points, List<Vector3> vertices, List<int> triangles, List<Color> vertexColors, 
            float offset, float height, float widthTop, float widthBottom, bool buildTop, bool buildBottom, 
            Color topColor, Color bottomColor,
            List<SplineWallHole.Data> holes, List<int> hiddenSegments)
        {
            /*for (int i = 0; i < points.Count - 1; i++)
            {
                if (hiddenSegments.Contains(i)) continue;
                
                List<SplineWallHole.Data> holesEdited = new List<SplineWallHole.Data>();
                foreach (SplineWallHole.Data h in holes)
                {
                    SplineWallHole.Data hole = h;
                    hole.yPos -= offset;
                    holesEdited.Add(hole);
                }
                
                MeshBuildUtilities.WallBuildData data = new MeshBuildUtilities.WallBuildData();
                data.isClosed = SplineInstance.isClosed;
                
                data.from = points[i] + Vector3.up * offset;
                data.to = points[i + 1] + Vector3.up * offset;

                data.buildTop = buildTop;
                data.buildBottom = buildBottom;

                SplineSample sampleFrom = new SplineSample();
                SplineInstance.Project(transform.TransformPoint(points[i]), ref sampleFrom);
                data.fromDistance = SplineInstance.CalculateLength(0f, sampleFrom.percent);

                SplineSample sampleTo = new SplineSample();
                SplineInstance.Project(transform.TransformPoint(points[i + 1]), ref sampleTo);
                
                data.toDistance = SplineInstance.CalculateLength(0f, i == points.Count - 2 ? 1f : sampleTo.percent);
                data.fullDistance = SplineInstance.CalculateLength(1f);

                data.widthTop = widthTop;
                data.widthBottom = widthBottom;
                data.height = height;
                data.vertOffset = vertices.Count;
                data.verts = vertices;
                data.tris = triangles;
                data.vertexColors = vertexColors;
                data.holes = holesEdited;
                
                data.topColor = topColor;
                data.bottomColor = bottomColor;

                int prevIndex = (i == 0) ? (points.Count - 2) : (i - 1); 
                data.prev = points[prevIndex] + Vector3.up * offset;
                data.usePrev = (i > 0 || SplineInstance.isClosed) && !hiddenSegments.Contains(prevIndex);
                
                int nextIndex = (i >= points.Count - 2) ? (1) : (i + 2);
                data.next = points[nextIndex] + Vector3.up * offset;
                data.useNext = (i < points.Count - 2 || SplineInstance.isClosed) && !hiddenSegments.Contains(nextIndex - 1);

                MeshBuildUtilities.BuildWall(data);
            }*/
        }

        private List<SplineWallHole.Data> GetHoles()
        {
            List<SplineWallHole.Data> holes = new List<SplineWallHole.Data>();
            GetComponentsInChildren<SplineWallHole>().ToList().ForEach(x => holes.Add(x.GetData()));
            return holes;
        }

        private Mesh GetMesh()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null || !mesh.name.Equals(gameObject.GetInstanceID().ToString()))
            {
                mesh = new Mesh();
                mesh.name = gameObject.GetInstanceID().ToString();
                meshFilter.sharedMesh = mesh;
            }

            return mesh;
        }

        public class WallSegment
        {
            public readonly Vector3 Min;
            public readonly Vector3 Max;

            public bool Hidden;

            public WallSegment Next;
            public WallSegment Prev;
            public WallSegment Upper;
            public WallSegment Lower;

            public WallSegment(Vector3 min, Vector3 max)
            {
                Min = min;
                Max = max;
            }
        }

        public class PercentInterval
        {
            public readonly double From;
            public readonly double To;
            
            public PercentInterval(double from, double to)
            {
                From = from;
                To = to;
            }

            public bool Contains(double percent)
            {
                if (From <= To)
                {
                    return percent > From && percent < To;
                }
                else
                {
                    return percent > From || percent < To;
                }
            }
        }
    }
}
#endif