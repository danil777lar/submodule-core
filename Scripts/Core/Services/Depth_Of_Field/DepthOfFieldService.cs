using UnityEngine;
using Larje.Core.Tools.CompositeProperties;
using Larje.Core;
using System;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

[BindService(typeof(DepthOfFieldService))]
public class DepthOfFieldService : Service
{
    public PriotizedProperty<DepthValue> Depth;

    [SerializeField] private bool autoDepth;
    [SerializeField] private LayerMask autoDepthLayerMask;
    [SerializeField] private float autoDepthStartMultiplier = 1.1f;
    [SerializeField] private float autoDepthEndMultiplier = 2f;
    [SerializeField] private float maxDistance = 10f;
    [Space]
    [SerializeField] private float speedStart = 10f;
    [SerializeField] private float speedEnd = 5f;

    private Camera _camera;
    private Volume _volume;

    public override void Init()
    {
        Depth = new PriotizedProperty<DepthValue>();
        if (autoDepth)
        {
            Depth.AddValue(GetDefaultDepth, () => 0);
        }
    }

    private void Update()
    {
        Volume volume = GetVolume();
        if (volume.profile.TryGet(out DepthOfField dof))
        {
            if (Depth.TryGetValue(out DepthValue depthValue))
            {
                dof.active = depthValue.Active;
                dof.gaussianStart.value = Mathf.Lerp(dof.gaussianStart.value, depthValue.Start, Time.deltaTime * speedStart);
                dof.gaussianEnd.value = Mathf.Lerp(dof.gaussianEnd.value, depthValue.End, Time.deltaTime * speedEnd);
            }
            else
            {
                dof.active = false;
            }
        }
    }

    private DepthValue GetDefaultDepth()
    {
        DepthValue depthValue = new DepthValue(); 

        depthValue.Active = !GetCamera().orthographic;
        if (depthValue.Active)
        {
            List<float> distances = new List<float>();

            distances.Add(GetDistance(Vector2.one * 0.5f));

            int raysCount = 4;
            for (int i = 0; i < raysCount; i++)
            {
                float angle = (360f / raysCount) * i;
                Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                Vector2 screenPercent = Vector2.one * 0.5f + direction * 0.1f;
                distances.Add(GetDistance(screenPercent));
            }
            distances = distances.OrderBy(x => x).ToList();

            float closestDistance = distances.First();
            if (closestDistance > maxDistance)
            {
                depthValue.Start = 1000f;
                depthValue.End = 1000f;
                return depthValue;
            }

            depthValue.Start = (distances.Max() + 2f) * autoDepthStartMultiplier;
            depthValue.End = (distances.Max() + 2f) * autoDepthEndMultiplier;
        }

        return depthValue;
    }

    private float GetDistance(Vector2 screenPercent)
    {
        Camera camera = GetCamera();
        Vector3 screenPoint = new Vector3(screenPercent.x * camera.pixelWidth, screenPercent.y * camera.pixelHeight, camera.nearClipPlane);
        Ray ray = camera.ScreenPointToRay(screenPoint);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, autoDepthLayerMask))
        {
            return hitInfo.distance;
        }

        return 1000f;
    }

    private Camera GetCamera()
    {
        if (_camera != null)
        {
            return _camera;
        }

        _camera = Camera.main;

        return _camera;
    }

    private Volume GetVolume()
    {
        if (_volume != null)
        {
            return _volume;
        }

        if (_volume == null)
        {
            _volume = FindObjectOfType<Volume>();
        }

        return _volume;
    }

    public struct DepthValue
    {
        public bool Active;
        public float Start;
        public float End;
    }
}
