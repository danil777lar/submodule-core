using System;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using UnityEngine;

public class VolumetricLightMesh : MonoBehaviour
{
    [SerializeField] private bool buildOnStart = true;
    [Space]
    [SerializeField] private Vector3 localDirection = Vector3.forward;
    [Space]
    [SerializeField] private float length = 5f;
    [SerializeField, Min(3)] private int corners = 4;
    [Space]
    [SerializeField] private Vector2 sizeStart;
    [SerializeField] private Vector2 sizeEnd;
    [SerializeField] private Color defaultColor;
    [Space]
    [SerializeField] private List<QualityMaterial> qualityMaterials;

    [InjectService] private IDataService _dataService;

    public void SetColors(Color color)
    {
        MeshFilter filter = GetMeshFilter();
        Mesh mesh = filter.sharedMesh;

        if (mesh != null)
        {
            int vertCount = mesh.vertices.Length;
            Color[] colors = new Color[vertCount];
            for (int i = 0; i < vertCount; i++)
            {
                colors[i] = color;
            }

            mesh.colors = colors;
        }
    }

    public void GetMeshData(out Vector3[] vertices, out int[] triangles, out Vector2[] uvs, out Color[] colors)
    {
        vertices = new Vector3[corners * 2];
        triangles = new int[corners * 6];
        uvs = new Vector2[corners * 2];
        colors = new Color[corners * 2];

        GetDirectionVectors(out Vector3 forward, out Vector3 right, out Vector3 up);

        for (int i = 0; i < corners; i++)
        {
            float angleRad = Mathf.Deg2Rad * ((i * 360f / corners) + 45f);
            float cos = Mathf.Cos(angleRad);
            float sin = Mathf.Sin(angleRad);

            float uvX = (float)i / corners;

            Vector3 startPoint = (right * cos * sizeStart.x) + (up * sin * sizeStart.y);
            vertices[i] = startPoint;
            uvs[i] = new Vector2(uvX, 0f);

            Vector3 endPoint = startPoint + forward * length;
            endPoint += (right * cos * sizeEnd.x) + (up * sin * sizeEnd.y);
            vertices[i + corners] = endPoint;
            uvs[i + corners] = new Vector2(uvX, 1f);
            colors[i] = defaultColor;
        }

        for (int i = 0; i < corners; i++)
        {
            int nextI = (i + 1) % corners;

            triangles[i * 6 + 0] = i;
            triangles[i * 6 + 1] = i + corners;
            triangles[i * 6 + 2] = nextI;

            triangles[i * 6 + 3] = nextI;
            triangles[i * 6 + 4] = i + corners;
            triangles[i * 6 + 5] = nextI + corners;
        }
    }

    private void Start()
    {
        DIContainer.InjectTo(this);
        if (buildOnStart)
        {
            GenerateMesh();

            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = GetMaterial();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        GetMeshData(out Vector3[] vertices, out int[] triangles, out Vector2[] uvs, out Color[] colors);

        Gizmos.color = Color.white;
        for (int i = 0; i < vertices.Length / 2; i++)
        {
            int vCount = vertices.Length / 2;
            Vector3 v0 = transform.TransformPoint(vertices[i]);
            Vector3 v1 = transform.TransformPoint(vertices[(i + 1) % vCount]);

            Vector3 v2 = transform.TransformPoint(vertices[i + vCount]);
            Vector3 v3 = transform.TransformPoint(vertices[(i + 1) % vCount + vCount]);

            Gizmos.DrawLine(v0, v1);
            Gizmos.DrawLine(v0, v2);
            Gizmos.DrawLine(v2, v3);
        }
    }

    
    [ContextMenu("Generate Mesh")]
    private void GenerateMesh()
    {
        MeshFilter meshFilter = GetMeshFilter();
        Mesh mesh = new Mesh();
        mesh.name = "Volumetric Light Mesh";

        GetMeshData(out Vector3[] vertices, out int[] triangles, out Vector2[] uvs, out Color[] colors);

        List<Vector4> posUvs = new List<Vector4>();
        for (int i = 0; i < vertices.Length; i++)
        {
            posUvs.Add(transform.localPosition);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.SetUVs(2, posUvs.ToArray());
        mesh.colors = colors;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    private MeshFilter GetMeshFilter()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        return meshFilter;
    }

    private void GetDirectionVectors(out Vector3 forward, out Vector3 right, out Vector3 up)
    {
        forward = localDirection.normalized;
        right = Vector3.Cross(forward, Vector3.up).normalized;
        up = Vector3.Cross(right, forward).normalized;
    }

    private Material GetMaterial()
    {
        VolumetricLightQuality quality = _dataService.SystemData.Settings.Graphics.VolumetricLightQuality;
        return qualityMaterials.Find(qm => qm.Quality == quality)?.Material;
    }

    [Serializable]
    private class QualityMaterial
    {
        [field: SerializeField] public VolumetricLightQuality Quality { get; private set; }
        [field: SerializeField] public Material Material { get; private set; }
    }
}
