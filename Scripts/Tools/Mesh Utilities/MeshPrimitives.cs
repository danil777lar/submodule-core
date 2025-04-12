using System.Collections.Generic;
using UnityEngine;

public static class MeshPrimitives
{
    public static Mesh GenerateCapsuleMesh(float radius, float height, int segments = 8, int rings = 4)
    {
        List<Vector3> vertices = new();
        List<Vector3> normals = new();
        List<int> triangles = new();

        float cylinderHeight = height - 2f * radius;
        cylinderHeight = Mathf.Max(0, cylinderHeight);

        int vertPerRing = segments + 1;

        // === ВЕРХНЯЯ ПОЛУСФЕРА ===
        for (int y = 0; y <= rings; y++)
        {
            float v = y / (float)rings;
            float phi = Mathf.PI / 2f * (1 - v); // от π/2 до 0
            float yPos = Mathf.Sin(phi) * radius + cylinderHeight / 2f;
            float ringRadius = Mathf.Cos(phi) * radius;

            for (int i = 0; i <= segments; i++)
            {
                float u = i / (float)segments;
                float theta = u * Mathf.PI * 2f;

                float x = Mathf.Cos(theta) * ringRadius;
                float z = Mathf.Sin(theta) * ringRadius;

                Vector3 pos = new Vector3(x, yPos, z);
                Vector3 normal = new Vector3(x, yPos - cylinderHeight / 2f, z).normalized;

                vertices.Add(pos);
                normals.Add(normal);
            }
        }

        // === ЦИЛИНДР ===
        for (int y = 0; y <= 1; y++)
        {
            float yPos = cylinderHeight / 2f - y * cylinderHeight;

            for (int i = 0; i <= segments; i++)
            {
                float u = i / (float)segments;
                float theta = u * Mathf.PI * 2f;

                float x = Mathf.Cos(theta) * radius;
                float z = Mathf.Sin(theta) * radius;

                Vector3 pos = new Vector3(x, yPos, z);
                Vector3 normal = new Vector3(x, 0, z).normalized;

                vertices.Add(pos);
                normals.Add(normal);
            }
        }

        // === НИЖНЯЯ ПОЛУСФЕРА ===
        for (int y = 1; y <= rings; y++) // y = 1, чтобы не дублировать цилиндровое кольцо
        {
            float v = y / (float)rings;
            float phi = Mathf.PI / 2f * v; // от 0 до π/2
            float yPos = -Mathf.Sin(phi) * radius - cylinderHeight / 2f;
            float ringRadius = Mathf.Cos(phi) * radius;

            for (int i = 0; i <= segments; i++)
            {
                float u = i / (float)segments;
                float theta = u * Mathf.PI * 2f;

                float x = Mathf.Cos(theta) * ringRadius;
                float z = Mathf.Sin(theta) * ringRadius;

                Vector3 pos = new Vector3(x, yPos, z);
                Vector3 normal = new Vector3(x, yPos + cylinderHeight / 2f, z).normalized;

                vertices.Add(pos);
                normals.Add(normal);
            }
        }

        // === ТРЕУГОЛЬНИКИ ===
        int ringsTotal = rings * 2 + 2; // верхняя + цилиндр (2) + нижняя
        for (int y = 0; y < ringsTotal; y++)
        {
            for (int i = 0; i < segments; i++)
            {
                int current = y * vertPerRing + i;
                int next = current + vertPerRing;

                triangles.Add(current);
                triangles.Add(next);
                triangles.Add(current + 1);

                triangles.Add(current + 1);
                triangles.Add(next);
                triangles.Add(next + 1);
            }
        }

        // === СОЗДАНИЕ МЕША ===
        Mesh mesh = new Mesh();
        mesh.name = "Perfect Capsule";
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();

        return mesh;
    }
}
