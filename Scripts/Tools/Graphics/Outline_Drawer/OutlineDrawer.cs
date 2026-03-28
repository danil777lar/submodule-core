using System;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Tools.CompositeProperties;
using UnityEngine;

public class OutlineDrawer : MonoBehaviour
{
    private const string ShaderName = "SubmoduleCore/URP/Outline_Unlit";
    private static Dictionary<Mesh, Mesh> _smoothMeshes;

    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private float defaultWidth = 0.05f;
    [SerializeField] private float defaultValue = 1f;
    [SerializeField] private float defaultPower = 1f;

    private Material _material;
    private RenderParams _params;
    private MeshFilter[] _meshFilters;

    public PriotizedProperty<Color> OutlineColor { get; } = new PriotizedProperty<Color>();
    public PriotizedProperty<float> OutlineWidth { get; } = new PriotizedProperty<float>();
    public PriotizedProperty<float> OutlineColorValue { get; } = new PriotizedProperty<float>();
    public FloatProduct OutlineColorPower { get; } = new FloatProduct();
    public FloatProduct OutlineWidthMultiplier { get; } = new FloatProduct();

    private void Start()
    {
        _meshFilters = GetComponentsInChildren<MeshFilter>();
        
        Shader shader = Shader.Find(ShaderName); 
        if (shader == null)
        {
            Debug.LogError($"Shader '{ShaderName}' not found. Please ensure it is included in the project.");
            enabled = false;
            return;
        }

        _material = new Material(shader);
        _params = new RenderParams(_material);
        _params.worldBounds = new Bounds(Vector3.zero, Vector3.one * 1000f);

        OutlineColor.AddValue(() => defaultColor, () => 0);
        OutlineWidth.AddValue(() => defaultWidth, () => 0);
        OutlineColorPower.AddValue(() => defaultPower);
        OutlineColorValue.AddValue(() => defaultValue, () => 0);
    }

    private void Update()
    {        
        float totalWidth = OutlineWidth.TryGetValue(out float width) ? width : defaultWidth;
        totalWidth *= OutlineWidthMultiplier.Value;

        _material.SetFloat("_Width", totalWidth);
        _material.SetColor("_Color", OutlineColor.TryGetValue(out Color color) ? color : defaultColor);
        _material.SetFloat("_ColorPower", OutlineColorPower.Value);
        _material.SetFloat("_ColorValue", OutlineColorValue.TryGetValue(out float value) ? value : defaultPower);

        foreach (MeshFilter meshFilter in _meshFilters)
        {
            DrawMesh(meshFilter);
        }
    }

    private void DrawMesh(MeshFilter meshFilter)
    {
        Mesh mesh = GetSmoothMesh(meshFilter.sharedMesh);

        Vector3 position = meshFilter.transform.position;
        Quaternion rotation = meshFilter.transform.rotation;
        Vector3 scale = meshFilter.transform.lossyScale;

        Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);
        Graphics.RenderMesh(_params, mesh, 0, matrix);
    }

    private Mesh GetSmoothMesh(Mesh mesh)
    {
        if (_smoothMeshes == null)
        {
            _smoothMeshes = new Dictionary<Mesh, Mesh>();
        }

        if (_smoothMeshes.TryGetValue(mesh, out Mesh smoothMesh))
        {
            return smoothMesh;
        }

        smoothMesh = Instantiate(mesh);
        SmoothMesh(smoothMesh);
        _smoothMeshes[mesh] = smoothMesh;

        return smoothMesh;
    }

    private void SmoothMesh(Mesh mesh, float weldTolerance = 0.0001f)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        int vertexCount = vertices.Length;
        int triangleCount = triangles.Length / 3;

        var faceNormals = new Vector3[triangleCount];
        var vertexToFaces = new List<int>[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            vertexToFaces[i] = new List<int>(6);
        }

        for (int tri = 0; tri < triangles.Length; tri += 3)
        {
            int faceIndex = tri / 3;

            int i0 = triangles[tri];
            int i1 = triangles[tri + 1];
            int i2 = triangles[tri + 2];

            Vector3 v0 = vertices[i0];
            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];

            Vector3 e1 = v1 - v0;
            Vector3 e2 = v2 - v0;

            Vector3 faceNormal = Vector3.Cross(e1, e2);
            float sqrMag = faceNormal.sqrMagnitude;

            if (sqrMag > 1e-12f)
            {
                faceNormal /= Mathf.Sqrt(sqrMag);
            }
            else
            {
                faceNormal = Vector3.up;
            }

            faceNormals[faceIndex] = faceNormal;

            vertexToFaces[i0].Add(faceIndex);
            vertexToFaces[i1].Add(faceIndex);
            vertexToFaces[i2].Add(faceIndex);
        }

        var groups = new Dictionary<VertexKey, List<int>>(vertexCount);

        for (int i = 0; i < vertexCount; i++)
        {
            var key = new VertexKey(vertices[i], weldTolerance);

            if (!groups.TryGetValue(key, out var list))
            {
                list = new List<int>(4);
                groups.Add(key, list);
            }

            list.Add(i);
        }

        var smoothNormals = new Vector3[vertexCount];

        foreach (var pair in groups)
        {
            List<int> group = pair.Value;
            Vector3 sum = Vector3.zero;

            for (int gi = 0; gi < group.Count; gi++)
            {
                int vertexIndex = group[gi];
                List<int> connectedFaces = vertexToFaces[vertexIndex];

                for (int fi = 0; fi < connectedFaces.Count; fi++)
                {
                    sum += faceNormals[connectedFaces[fi]];
                }
            }

            Vector3 smooth = sum.sqrMagnitude > 1e-12f ? sum.normalized : Vector3.up;

            for (int gi = 0; gi < group.Count; gi++)
            {
                smoothNormals[group[gi]] = smooth;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = smoothNormals;
    }

    private readonly struct VertexKey : IEquatable<VertexKey>
    {
        private readonly int x;
        private readonly int y;
        private readonly int z;

        public VertexKey(Vector3 v, float tolerance)
        {
            x = Mathf.RoundToInt(v.x / tolerance);
            y = Mathf.RoundToInt(v.y / tolerance);
            z = Mathf.RoundToInt(v.z / tolerance);
        }

        public bool Equals(VertexKey other) => x == other.x && y == other.y && z == other.z;

        public override bool Equals(object obj) => obj is VertexKey other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = x;
                hash = hash * 486187739 + y;
                hash = hash * 486187739 + z;
                return hash;
            }
        }
    }
}
