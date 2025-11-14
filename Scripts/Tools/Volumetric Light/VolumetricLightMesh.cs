using UnityEngine;

public class VolumetricLightMesh : MonoBehaviour
{
    [SerializeField] private Vector3 localDirection = Vector3.forward;
    [Space]
    [SerializeField] private float length = 5f;
    [SerializeField] private float angle = 30f;
    [SerializeField, Min(3)] private int corners = 4;
    [Space]
    [SerializeField] private Vector2 sizeStart;
    [SerializeField] private Vector2 sizeEnd;

    private void OnDrawGizmosSelected()
    {
        GetDirectionVectors(out Vector3 forward, out Vector3 right, out Vector3 up);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(forward) * length);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(right) * sizeStart.x);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(up) * sizeStart.y);
    }
    
    [ContextMenu("Generate Mesh")]
    private void GenerateMesh()
    {
        GetDirectionVectors(out Vector3 forward, out Vector3 right, out Vector3 up);

        MeshFilter meshFilter = GetMeshFilter();
        Mesh mesh = new Mesh();
        mesh.name = "Volumetric Light Mesh";

        Vector3[] vertices = new Vector3[corners * 2];
        int[] triangles = new int[vertices.Length * 3];

        for (int i = 0; i < corners; i++)
        {
            float angleRad = Mathf.Deg2Rad * ((i * 360f / corners) + 45f);
            float cos = Mathf.Cos(angleRad);
            float sin = Mathf.Sin(angleRad);

            Vector3 startPoint = (right * cos * sizeStart.x) + (up * sin * sizeStart.y);
            vertices[i] = startPoint;

            Vector3 endPoint = startPoint + forward * length;
            endPoint += (right * cos * sizeEnd.x) + (up * sin * sizeEnd.y);
            vertices[i + corners] = endPoint;
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

        mesh.vertices = vertices;
        mesh.triangles = triangles;
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
}

