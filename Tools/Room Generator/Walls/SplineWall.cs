#if DREAMTECK_SPLINES

using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using MoreMountains.Tools;
using UnityEngine;

namespace Larje.Core.Tools.RoomGenerator
{
    [RequireComponent(typeof(SplineComputer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class SplineWall : MonoBehaviour
    {
        [SerializeField] private SplineWallConfig config;
        [Space]
        [SerializeField, Min(0f)] private float segmentsPerUnit = 0f;
        [SerializeField] private bool rebuildOnStart = false;
        [Space]
        [SerializeField] private List<bool> hideWallParts = new List<bool>();

        private SplineComputer _spline;

        public float SegmentsPerUnit => segmentsPerUnit;
        public SplineWallConfig Config => config;
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
        public IReadOnlyCollection<bool> HideWallParts => hideWallParts;

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
            
            foreach (WallSegment segment in WallSegmentUtilities.GetSegments(this))
            {
                Gizmos.color = segment.Hidden ? Color.white.SetAlpha(0.1f) : Color.white.SetAlpha(1f);
                
                //diagonal lines
                Gizmos.DrawLine(segment.Min,  segment.Max);
                Gizmos.DrawLine(segment.Min.MMSetY(segment.Max.y),  segment.Max.MMSetY(segment.Min.y));
                
                //horizontal lines
                Gizmos.DrawLine(segment.Min, segment.Max.MMSetY(segment.Min.y));
                Gizmos.DrawLine(segment.Max, segment.Min.MMSetY(segment.Max.y));
                
                //vertical lines
                Gizmos.DrawLine(segment.Min,  segment.Min.MMSetY(segment.Max.y));
                Gizmos.DrawLine(segment.Max,  segment.Max.MMSetY(segment.Min.y));
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
    }
}
#endif