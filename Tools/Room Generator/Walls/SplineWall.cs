#if DREAMTECK_SPLINES

using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using MoreMountains.Tools;
using UnityEngine;

namespace Larje.Core.Tools.RoomGenerator
{
    [RequireComponent(typeof(SplineComputer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class SplineWall : MonoBehaviour
    {
        [SerializeField] private SplineWallConfig config;
        [SerializeField, Min(0f)] private float segmentsPerUnit = 0f;
        [SerializeField] private bool rebuildOnStart = false;
        [Space]
        [SerializeField] private List<bool> hideWallParts = new List<bool>();
        [Header("Debug")]
        [SerializeField] private bool enableDebugMode;
        [SerializeField] private bool drawGizmos;

        private SplineComputer _spline;
        
        public bool EnableDebugMode => enableDebugMode;
        public float SegmentsPerUnit => segmentsPerUnit;
        public SplineWallConfig Config => config;
        public SplineComputer SplineInstance
        {
            get
            {
                if (_spline == null)
                {
                    _spline = GetComponent<SplineComputer>();
                }

                return _spline;
            }
        }
        public IReadOnlyCollection<bool> HideWallParts => hideWallParts;

        public void Initialize()
        {
            Rebuild();
        }

        public void Initialize(Material wallBottomMaterial, Material wallTopMaterial)
        {
            Rebuild();
            MeshRenderer rend = GetComponent<MeshRenderer>();
            rend.materials = new Material[] { wallBottomMaterial, wallTopMaterial };
        }

        private void Start()
        {
            if (rebuildOnStart)
            {
                Rebuild();
            }
        }

        private void OnDrawGizmos()
        {
            if (drawGizmos)
            {
                DrawWallPointsGizmo();
                DrawWallSegmentsGizmo();
            }

            if (!Application.isPlaying)
            {
                Rebuild();
            }
        }

        private void OnValidate()
        {
            ValidateHideWallParts();
        }
        
        private void DrawWallPointsGizmo()
        {
            foreach (double point in WallPointUtilities.GetPercentPoint(this))
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(SplineInstance.EvaluatePosition(point), 0.1f);
            }
        }

        private void DrawWallSegmentsGizmo()
        {
            foreach (WallSegment segment in WallSegmentUtilities.GetSegments(this))
            {
                Vector3 min = transform.TransformPoint(segment.Min);
                Vector3 max = transform.TransformPoint(segment.Max);
                Vector3 offsetMin = transform.TransformVector(segment.OffsetFrom);
                Vector3 offsetMax = transform.TransformVector(segment.OffsetTo);
                
                Gizmos.color = segment.Hidden ? Color.white.SetAlpha(0.1f) : Color.white.SetAlpha(1f);
                
                //diagonal lines
                Gizmos.DrawLine(min,  max);
                Gizmos.DrawLine(min.MMSetY(max.y),  max.MMSetY(min.y));
                
                //horizontal lines
                Gizmos.DrawLine(min, max.MMSetY(min.y));
                Gizmos.DrawLine(max, min.MMSetY(max.y));
                
                //vertical lines
                Gizmos.DrawLine(min,  min.MMSetY(max.y));
                Gizmos.DrawLine(max,  max.MMSetY(min.y));
                
                //offsets
                Gizmos.DrawLine(min - offsetMin,  min + offsetMin);
                Gizmos.DrawLine(max.MMSetY(min.y) - offsetMax,  max.MMSetY(min.y) + offsetMax);
            }
        }

        private void ValidateHideWallParts()
        {
            if (SplineInstance != null)
            {
                while (hideWallParts.Count < SplineInstance.GetPoints().Length)
                {
                    hideWallParts.Add(false);
                }

                while (hideWallParts.Count > SplineInstance.GetPoints().Length)
                {
                    hideWallParts.RemoveAt(hideWallParts.Count - 1);
                }
            }
        }

        private void Rebuild()
        {
            if (CanRebuild())
            {
                WallMeshUtilities.BuildMesh(this, GetMesh());
                ApplyMaterials(config.WallParts.Select(x => x.Material).ToArray());   
                ApplyCollider();
            }
        }

        private bool CanRebuild()
        {
            return SplineInstance != null && config != null && config.WallParts.Length > 0;
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
        
        private void ApplyMaterials(Material[] materials)
        {
            MeshRenderer rend = GetComponent<MeshRenderer>();
            rend.sharedMaterials = materials.ToArray();
        }

        private void ApplyCollider()
        {
            Mesh mesh = GetMesh();
            if (mesh.vertices.Length > 0)
            {
                MeshCollider collider = GetComponent<MeshCollider>();
                collider.sharedMesh = mesh;
            }
        }
    }
}
#endif