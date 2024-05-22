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
        [Header("Radius")] 
        [SerializeField] private bool radiusToEdge; 
        [SerializeField] private float radius = 5;
        [Space]
        [SerializeField, Range(0f, 360f)] private float rotate;
        [SerializeField] private Vector2 scale = new Vector2(1f, 1f);
        [SerializeField, Min(3)] private int wallsCount = 3;
        [SerializeField] private bool rebuildOnStart = false;
        [Header("Walls")]
        [SerializeField] private float bottomWallHeight;
        [SerializeField] private SplineWall mainWall;
        [SerializeField] private SplineWall bottomWall;

        private MeshFilter _meshFilter;

        private MeshFilter MeshFilter
        {
            get
            {
                if (_meshFilter == null)
                {
                    _meshFilter = GetComponent<MeshFilter>();
                }

                return _meshFilter;
            }
        }
        
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
        }//

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
            
            MeshCollider collider = GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }

        private List<Vector3> GetCorners()
        {
            List<Vector3> vertices = new List<Vector3>();
            for (int i = 0; i < wallsCount; i++)
            {
                float angleStep = 360f / wallsCount; 
                float angle = (angleStep * i + rotate) * Mathf.Deg2Rad;
                float distance = radius;
                if (radiusToEdge)
                {
                    float a = 180f - (angleStep / 2) - 90f;
                    distance = radius / Mathf.Sin(a * Mathf.Deg2Rad);
                }
                
                Vector3 vert = new Vector3(Mathf.Cos(angle) * scale.x, 0f, Mathf.Sin(angle) * scale.y) * distance;
                vertices.Add(vert);
            }

            return vertices;
        }

        private Mesh GetMesh()
        {
            Mesh mesh = MeshFilter.sharedMesh;
            if (mesh == null || !mesh.name.Equals(gameObject.GetInstanceID().ToString()))
            {
                mesh = new Mesh();
                mesh.name = gameObject.GetInstanceID().ToString();
                MeshFilter.sharedMesh = mesh;
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