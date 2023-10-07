#if UNITY_2022_3_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

namespace Larje.Core.Tools.RoomGenerator
{
    [RequireComponent(typeof(SplineContainer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class SplineWall : MonoBehaviour
    {
        [SerializeField] private bool rebuildOnStart = false;

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

        private void OnDrawGizmosSelected()
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

            SplineContainer spline = GetComponent<SplineContainer>();

            if (spline.Spline == null)
            {
                return;
            }

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            List<Vector3> points = new List<Vector3>();
            BezierKnot[] knots = spline.Spline.ToArray();

            for (int i = 0; i < knots.Length; i++)
            {
                points.Add(knots[i].Position);
            }

            float height = RoomMeshConfig.Instance.WallHeight;
            float percent = RoomMeshConfig.Instance.WallDividePercent;

            SubMeshDescriptor subMeshBottom = new SubMeshDescriptor();
            subMeshBottom.indexStart = triangles.Count;
            BuildWall(points, vertices, triangles, 0, height * percent);
            subMeshBottom.indexCount = triangles.Count - subMeshBottom.indexStart;

            SubMeshDescriptor subMeshTop = new SubMeshDescriptor();
            subMeshTop.indexStart = triangles.Count;
            BuildWall(points, vertices, triangles, height * percent, height * (1f - percent));
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

            MeshCollider collider = GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }

        private void BuildWall(List<Vector3> points, List<Vector3> vertices, List<int> triangles, float offset,
            float height)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                MeshBuildUtilities.WallBuildData data = new MeshBuildUtilities.WallBuildData();
                data.from = points[i] + Vector3.up * offset;
                data.to = points[i + 1] + Vector3.up * offset;
                data.width = RoomMeshConfig.Instance.WallWidth;
                data.height = height;
                data.vertOffset = vertices.Count;
                data.verts = vertices;
                data.tris = triangles;

                if (i > 0)
                {
                    data.usePrev = true;
                    data.prev = points[i - 1] + Vector3.up * offset;
                }

                if (i < points.Count - 2)
                {
                    data.useNext = true;
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