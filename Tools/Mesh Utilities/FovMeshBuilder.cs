using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FovMeshBuilder
{
    public readonly Options options;
    
    private float angleStep;
    private List<RaycastHit> _hits;
    private List<Vector3> _verts;
    private Dictionary<Vector3, float> _vertAngles;
    
    public float AngleStep => angleStep;
    public IReadOnlyCollection<RaycastHit> Hits => _hits;
    public IReadOnlyCollection<Vector3> Verts => _verts;
    public IReadOnlyDictionary<Vector3, float> VertAngles => _vertAngles;
    
    public FovMeshBuilder(Options options)
    {
        this.options = options;
    }
    
    public void BuildMesh()
    {
        _hits = new List<RaycastHit>();
        _verts = new List<Vector3>();
        _vertAngles = new Dictionary<Vector3, float>();

        if (options.raysPerDeg <= 0f)
        {
            return;
        }
        
        int rayCount = Mathf.RoundToInt(options.angle * options.raysPerDeg);
        float halfAngle = options.angle * 0.5f;
        
        angleStep = options.angle / rayCount;
        
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
            direction = Quaternion.Euler(0, options.directionRotate, 0) * direction;
            direction.Normalize();

            Vector3 raycastOrigin = options.meshFilter.transform.position + options.raycastOffset;
            Vector3 localDirection = options.meshFilter.transform.TransformDirection(direction);
            Vector3 vertex = Vector3.zero;

            if (Physics.Raycast(raycastOrigin, localDirection, out RaycastHit hit, options.distance, options.raycastMask))
            {
                vertex = options.meshFilter.transform.InverseTransformPoint(hit.point - options.raycastOffset);
                _hits.Add(hit);
            }
            else
            {
                vertex = direction * options.distance;
            }
            
            _verts.Add(vertex);
            _vertAngles.Add(vertex, halfAngle);

            verticles[vertexIndex] = vertex;
            if (i > 0)
            {
                triangles[triangleIndex + 0] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = 0;
                triangleIndex += 3;
            }

            vertexIndex++;
            halfAngle -= angleStep;
        }

        if (options.meshFilter.mesh == null)
        {
            options.meshFilter.mesh = new Mesh
            {
                name = "FOV"
            };
        }

        options.meshFilter.mesh.vertices = verticles;
        options.meshFilter.mesh.uv = uv;
        options.meshFilter.mesh.triangles = triangles;
    }

    public class Options
    {
        public float angle;
        public float raysPerDeg;
        public float distance;
        public float directionRotate;
        public Vector3 raycastOffset;
        public LayerMask raycastMask;
        public MeshFilter meshFilter;
    }
}
