using System.Linq;
using UnityEngine;

public class OutlineDrawer : MonoBehaviour
{
    [SerializeField] private Material material;

    private RenderParams _params;
    private MeshFilter[] _meshFilters;

    private void Start()
    {
        _meshFilters = GetComponentsInChildren<MeshFilter>();
        
        _params = new RenderParams(material);
        _params.worldBounds = new Bounds(Vector3.zero, Vector3.one * 1000f);
    }

    private void Update()
    {        
        foreach (MeshFilter meshFilter in _meshFilters)
        {
            DrawMesh(meshFilter);
        }
    }

    private void DrawMesh(MeshFilter meshFilter)
    {
        Mesh mesh = meshFilter.sharedMesh;

        Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0, 0, 5), Quaternion.identity, Vector3.one);
        Graphics.RenderMesh(_params, mesh, 0, matrix);
    }
}
