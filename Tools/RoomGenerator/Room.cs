#if DREAMTECK_SPLINES

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Larje.Core.Tools.RoomGenerator
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class Room : MonoBehaviour
    {
        [SerializeField] private float roomBottomHeight;
        [SerializeField] private float radius;
        [SerializeField, Range(0f, 360f)] private float rotate;
        [SerializeField] private Vector2 scale = new Vector2(1f, 1f);
        [SerializeField, Min(3)] private int wallsCount = 3;
        [SerializeField] private List<bool> doors;
        [SerializeField] private bool rebuildOnStart = false;
        [SerializeField] private Material floorMaterial;

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

        private void OnDrawGizmosSelected()
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
                ValidateDoorsList();
                BuildMesh(GetMesh());
            }
        }

        private void BuildMesh(Mesh mesh)
        {
            /*mesh.Clear();

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

            float height = RoomMeshConfig.Instance.WallHeight;
            float percent = RoomMeshConfig.Instance.WallDividePercent;
            float bottomHeight = RoomMeshConfig.Instance.RoomBottomHeight;

            SubMeshDescriptor subMeshBottom = new SubMeshDescriptor();
            subMeshBottom.indexStart = triangles.Count;
            BuildWalls(corners, vertices, triangles, -bottomHeight, (height * percent) + bottomHeight);
            subMeshBottom.indexCount = triangles.Count - subMeshBottom.indexStart;

            SubMeshDescriptor subMeshTop = new SubMeshDescriptor();
            subMeshTop.indexStart = triangles.Count;
            BuildWalls(corners, vertices, triangles, height * percent, height * (1f - percent));
            subMeshTop.indexCount = triangles.Count - subMeshTop.indexStart;

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.subMeshCount = 3;
            mesh.SetSubMeshes(new SubMeshDescriptor[]
            {
                subMeshFloor,
                subMeshBottom,
                subMeshTop
            });

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            MeshRenderer rend = GetComponent<MeshRenderer>();
            rend.sharedMaterials = new Material[]
            {
                RoomMeshConfig.Instance.FloorMaterial,
                RoomMeshConfig.Instance.WallBottomMaterial,
                RoomMeshConfig.Instance.WallTopMaterial
            };

            MeshCollider collider = GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;*/
        }

        private void BuildWalls(List<Vector3> corners, List<Vector3> vertices, List<int> triangles, float offset,
            float height)
        {
            /*for (int i = 0; i < corners.Count; i++)
            {
                int parts = doors[i] ? 2 : 1;
                for (int p = 0; p < parts; p++)
                {
                    MeshBuildUtilities.WallBuildData data = new MeshBuildUtilities.WallBuildData();
                    data.prev = corners[i == 0 ? corners.Count - 1 : i - 1] + Vector3.up * offset;
                    data.from = corners[i] + Vector3.up * offset;
                    data.to = corners[(i + 1) % corners.Count] + Vector3.up * offset;
                    data.next = corners[(i + 2) % corners.Count] + Vector3.up * offset;

                    if (doors[i])
                    {
                        Vector3 dir = data.to - data.from;
                        Vector3 center = data.from + dir * 0.5f;
                        if (p == 0)
                        {
                            data.next = data.to;
                            data.to = center - (dir.normalized * RoomMeshConfig.Instance.DoorWidth * 0.5f);
                        }
                        else
                        {
                            data.prev = data.from;
                            data.from = center + (dir.normalized * RoomMeshConfig.Instance.DoorWidth * 0.5f);
                        }
                    }

                    data.usePrev = true;
                    data.useNext = true;

                    data.width = RoomMeshConfig.Instance.WallWidth;
                    data.height = height;
                    data.vertOffset = vertices.Count;
                    data.verts = vertices;
                    data.tris = triangles;

                    MeshBuildUtilities.BuildWall(data);
                }
            }*/
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

        private void ValidateDoorsList()
        {
            doors ??= new List<bool>();

            while (doors.Count < wallsCount)
            {
                doors.Add(new bool());
            }

            while (doors.Count > wallsCount)
            {
                doors.RemoveAt(doors.Count - 1);
            }
        }
    }
}
#endif