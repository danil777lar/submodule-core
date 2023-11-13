#if DREAMTECK_SPLINES

using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Larje.Core.Tools.RoomGenerator
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class Room : MonoBehaviour
    {
        [SerializeField] private float radius = 5;
        [SerializeField, Range(0f, 360f)] private float rotate;
        [SerializeField] private Vector2 scale = new Vector2(1f, 1f);
        [SerializeField, Min(3)] private int wallsCount = 3;
        [SerializeField] private bool rebuildOnStart = false;
        [SerializeField] private Material floorMaterial;
        [Header("Walls")]
        [SerializeField] private float bottomWallHeight;
        [SerializeField] private SplineWall mainWall;
        [SerializeField] private SplineWall bottomWall;

        public float Radius => radius;

        public void Initialize()
        {
            BuildMesh(GetMesh());
        }

        public void Initialize(Material floorMaterial, Material wallBottomMaterial, Material wallTopMaterial)
        {
            BuildMesh(GetMesh());
            MeshRenderer rend = GetComponent<MeshRenderer>();
            rend.materials = new Material[] { floorMaterial, wallBottomMaterial, wallTopMaterial };
        }

        private void Start()
        {
            if (rebuildOnStart)
            {
                BuildMesh(GetMesh());
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                BuildMesh(GetMesh());
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                BuildMesh(GetMesh());
            }
        }

        private void BuildMesh(Mesh mesh)
        {
            ValidateWall();
            
            mesh.Clear();

            List<Vector3> corners = GetCorners();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            vertices.Add(Vector3.zero);
            vertices.AddRange(corners);

            SubMeshDescriptor subMeshFloor = new SubMeshDescriptor();
            subMeshFloor.indexStart = triangles.Count;
            for (int i = 0; i < wallsCount; i++)
            {
                triangles.Add(0);
                triangles.Add(i == wallsCount - 1 ? 1 : i + 2);
                triangles.Add(i + 1);
            }
            subMeshFloor.indexCount = triangles.Count - subMeshFloor.indexStart;
            
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.subMeshCount = 3;
            mesh.SetSubMeshes(new SubMeshDescriptor[] { subMeshFloor });

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            MeshRenderer rend = GetComponent<MeshRenderer>();
            rend.sharedMaterials = new Material[] { floorMaterial };

            MeshCollider collider = GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }

        private List<Vector3> GetCorners()
        {
            List<Vector3> vertices = new List<Vector3>();
            for (int i = 0; i < wallsCount; i++)
            {
                float angle = (360f / wallsCount * i + rotate) * Mathf.Deg2Rad;
                Vector3 vert = new Vector3(Mathf.Cos(angle) * scale.x, 0f, Mathf.Sin(angle) * scale.y) * radius;
                vertices.Add(vert);
            }

            return vertices;
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

        private void ValidateWall()
        {
            List<SplinePoint> splinePoints = new List<SplinePoint>();

            foreach (Vector3 point in GetCorners())
            {
                SplinePoint splinePoint = new SplinePoint();
                splinePoint.position = transform.TransformPoint(point);
                splinePoint.normal = Vector3.up;
                splinePoints.Add(splinePoint);
            }

            if (mainWall != null)
            {
                SetSplineValues(mainWall, splinePoints);
            }

            if (bottomWall != null)
            {
                Vector3 bottomPosition = transform.position;
                bottomPosition -= Vector3.up * bottomWallHeight;
                bottomWall.transform.position = bottomPosition; 
                SetSplineValues(bottomWall, splinePoints);
            }
        }

        private void SetSplineValues(SplineWall splineWall, List<SplinePoint> points)
        {
            splineWall.SplineInstance.SetPoints(points.ToArray());
            splineWall.SplineInstance.type = Spline.Type.Linear;
            splineWall.SplineInstance.Close();
        }
    }
}
#endif