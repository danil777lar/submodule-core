#if DREAMTECK_SPLINES

using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using Larje.Core.Services;
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
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                Rebuild();
            }
        }

        private void Rebuild()
        {
            Mesh mesh = GetMesh();

            if (SplineInstance == null || config == null || config.WallParts.Length == 0)
            {
                return;
            }

            List<SplineWallHole.Data> holes = new List<SplineWallHole.Data>();
            GetComponentsInChildren<SplineWallHole>().ToList().ForEach(x => holes.Add(x.GetData()));

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            
            List<Vector3> points = GetPoints();
            List<SubMeshDescriptor> submeshes = new List<SubMeshDescriptor>();
            List<Material> materials = new List<Material>();

            float weightsSum = config.GetWeightsSum();
            float lastHeights = 0f;
            foreach (SplineWallConfig.WallPart wallPart in config.WallParts)
            {
                float partHeightPercent = wallPart.Weight / weightsSum;
                float partOffset = lastHeights;
                float partHeight = config.Height * partHeightPercent;
                float partWidthTop = config.Width * wallPart.TopWidthMultiplier;
                float partWidthBottom = config.Width * wallPart.BottomWidthMultiplier;
                lastHeights += partHeight;
                
                materials.Add(wallPart.Material);
                
                SubMeshDescriptor submesh = new SubMeshDescriptor();
                submesh.indexStart = triangles.Count;
                BuildWall(points, vertices, triangles, partOffset, partHeight, partWidthTop, 
                    partWidthBottom, wallPart.DrawTop, wallPart.DrawBottom, holes);
                submesh.indexCount = triangles.Count - submesh.indexStart;
                submeshes.Add(submesh);
            }

            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

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
                points.Add(transform.InverseTransformPoint(splinePoints[i].position));
                float lengthToNext = SplineInstance.CalculateLength(SplineInstance.GetPointPercent(i),
                    SplineInstance.GetPointPercent(i + 1));
                int pointsCount = Mathf.RoundToInt(lengthToNext * segmentsPerUnit);
                for (int j = 1; j < pointsCount; j++)
                {
                    float percent = Mathf.Lerp(
                        (float) SplineInstance.GetPointPercent(i),
                        (float) SplineInstance.GetPointPercent(i + 1), 
                        (float)j / (float)pointsCount);
                    
                    
                    points.Add(transform.InverseTransformPoint(SplineInstance.EvaluatePosition(percent)));
                }
            }
            points.Add(transform.InverseTransformPoint(splinePoints.Last().position));

            for (int i = 0; i < points.Count; i++)
            {
                Vector3 fixedPoint = points[i];
                fixedPoint.y = 0f;
                points[i] = fixedPoint;
            }

            return points;
        }

        private void BuildWall(List<Vector3> points, List<Vector3> vertices, List<int> triangles, float offset,
            float height, float widthTop, float widthBottom, bool buildTop, bool buildBottom, List<SplineWallHole.Data> holes)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
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
                data.holes = holesEdited;

                data.usePrev = i > 0 || SplineInstance.isClosed;
                if (i == 0)
                {
                    data.prev = points[^2] + Vector3.up * offset;
                }
                else
                {
                    data.prev = points[i - 1] + Vector3.up * offset;
                }

                data.useNext = i < points.Count - 2 || SplineInstance.isClosed;
                if (i >= points.Count - 2)
                {
                    data.next = points[1] + Vector3.up * offset;
                }
                else
                {
                    data.next = points[i + 2] + Vector3.up * offset;
                }

                MeshBuildUtilities.BuildWall(data);
            }
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
    }
}
#endif