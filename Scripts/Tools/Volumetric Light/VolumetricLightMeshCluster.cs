using System.Collections.Generic;
using UnityEngine;

public class VolumetricLightMeshCluster : MonoBehaviour
{
    private void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>(); 
        VolumetricLightMesh[] lights = GetComponentsInChildren<VolumetricLightMesh>();

        Mesh mesh = new Mesh();
        mesh.name = "Volumetric Light Mesh";

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector4> uvs2 = new List<Vector4>();
        List<Color> colors = new List<Color>();

        foreach (VolumetricLightMesh light in lights)
        {
            light.GetMeshData(out Vector3[] lightVerts, out int[] lightTris, out Vector2[] lightUvs, out Color[] lightColors);

            int vertexOffset = vertices.Count;

            foreach (Vector3 v in lightVerts)
            {
                Vector3 worldPos = light.transform.TransformPoint(v);
                Vector3 localPos = transform.InverseTransformPoint(worldPos);

                vertices.Add(localPos);
                uvs2.Add(light.transform.localPosition);
            }

            foreach (int t in lightTris)
            {
                triangles.Add(t + vertexOffset);
            }

            uvs.AddRange(lightUvs);
            colors.AddRange(lightColors);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.SetUVs(2, uvs2.ToArray());
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }
}
