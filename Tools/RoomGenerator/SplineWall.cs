#if DREAMTECK_SPLINES

using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using Larje.Core.Services;
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
            
            if (SplineInstance == null)
            {
                return;
            }
            
            List<SplineWallHole.Data> holes = new List<SplineWallHole.Data>();
            GetComponentsInChildren<SplineWallHole>().ToList().ForEach(x => holes.Add(x.GetData()));
            
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            float height = RoomMeshConfig.Instance.WallHeight;
            float percent = RoomMeshConfig.Instance.WallDividePercent;

            List<SplinePoint> points = SplineInstance.GetPoints().ToList();

            SubMeshDescriptor subMeshBottom = new SubMeshDescriptor();
            subMeshBottom.indexStart = triangles.Count;
            BuildWall(points, vertices, triangles, 0, height * percent, holes);
            subMeshBottom.indexCount = triangles.Count - subMeshBottom.indexStart;

            SubMeshDescriptor subMeshTop = new SubMeshDescriptor();
            subMeshTop.indexStart = triangles.Count;
            BuildWall(points, vertices, triangles, height * percent, height * (1f - percent), holes);
            subMeshTop.indexCount = triangles.Count - subMeshTop.indexStart;

            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.subMeshCount = 2;
            mesh.SetSubMeshes(new SubMeshDescriptor[] { subMeshBottom, subMeshTop });

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            MeshRenderer rend = GetComponent<MeshRenderer>();
            rend.sharedMaterials = new Material[]
            {
                RoomMeshConfig.Instance.WallBottomMaterial,
                RoomMeshConfig.Instance.WallTopMaterial
            };

            if (mesh.vertices.Length > 0)
            {
                MeshCollider collider = GetComponent<MeshCollider>();
                collider.sharedMesh = mesh;   
            }
        }

        private void BuildWall(List<SplinePoint> points, List<Vector3> vertices, List<int> triangles, float offset,
            float height, List<SplineWallHole.Data> holes)
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
                data.from = points[i].position + Vector3.up * offset;
                data.to = points[i + 1].position + Vector3.up * offset;

                data.fromDistance = SplineInstance.CalculateLength(0f, SplineInstance.GetPointPercent(i)); 
                data.toDistance = SplineInstance.CalculateLength(0f, SplineInstance.GetPointPercent(i + 1));
                
                data.width = RoomMeshConfig.Instance.WallWidth;
                data.height = height;
                data.vertOffset = vertices.Count;
                data.verts = vertices;
                data.tris = triangles;
                data.holes = holesEdited;

                if (i > 0)
                {
                    data.usePrev = true;
                    data.prev = points[i - 1].position + Vector3.up * offset;
                }

                if (i < points.Count - 2)
                {
                    data.useNext = true;
                    data.next = points[i + 2].position + Vector3.up * offset;
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