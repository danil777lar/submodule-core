using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FovMeshBuilder
{
    public static Output BuildMesh(Input input)
    {
        Output output = new Output();
        output.verts = new List<Vector3>();
        output.hits = new List<RaycastHit>();

        if (input.raysPerDeg <= 0f)
        {
            return output;
        }
        
        int rayCount = Mathf.RoundToInt(input.angle * input.raysPerDeg);
        float halfAngle = input.angle * 0.5f;
        
        float angleIncrease = input.angle / rayCount;
        output.angleIncrease = angleIncrease;
        
        Vector3[] verticles = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[verticles.Length];
        int[] triangles = new int[rayCount * 3];

        verticles[0] = Vector3.zero;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 direction = Vector3.zero;
            direction.x = Mathf.Sin(Mathf.Deg2Rad * halfAngle);
            direction.z = Mathf.Cos(Mathf.Deg2Rad * halfAngle);
            direction = Quaternion.Euler(0, input.directionRotate, 0) * direction;
            direction.Normalize();

            Vector3 raycastOrigin = input.meshFilter.transform.position + input.raycastOffset;
            Vector3 localDirection = input.meshFilter.transform.TransformDirection(direction);
            Vector3 vertex = Vector3.zero;

            if (Physics.Raycast(raycastOrigin, localDirection, out RaycastHit hit, input.distance, input.raycastMask))
            {
                vertex = input.meshFilter.transform.InverseTransformPoint(hit.point - input.raycastOffset);
                output.hits.Add(hit);
            }
            else
            {
                vertex = direction * input.distance;
            }
            output.verts.Add(vertex);

            verticles[vertexIndex] = vertex;
            if (i > 0)
            {
                triangles[triangleIndex + 0] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = 0;
                triangleIndex += 3;
            }

            vertexIndex++;
            halfAngle -= angleIncrease;
        }

        if (input.meshFilter.mesh == null)
        {
            input.meshFilter.mesh = new Mesh
            {
                name = "FOV"
            };
        }

        input.meshFilter.mesh.vertices = verticles;
        input.meshFilter.mesh.uv = uv;
        input.meshFilter.mesh.triangles = triangles;
        
        return output;
    }

    public struct Input
    {
        public float angle;
        public float raysPerDeg;
        public float distance;
        public float directionRotate;
        public Vector3 raycastOffset;
        public LayerMask raycastMask;
        public MeshFilter meshFilter;
    }
    
    public struct Output
    {
        public float angleIncrease;
        public List<Vector3> verts;
        public List<RaycastHit> hits;
    }
}
