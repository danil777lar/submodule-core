using System;
using System.Collections.Generic;
using UnityEngine;

public class LookTriggerSender : TriggerSender
{
    [Space, Header("Look Settings")]
    [SerializeField] private Vector2 screenMin = Vector2.zero;
    [SerializeField] private Vector2 screenMax = Vector2.one;
    
    private Camera _mainCamera;

    protected override void Start()
    {
        base.Start();
        _mainCamera = Camera.main;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green.SetAlpha(0.5f);
        if (Value >= 1)
        {
            Gizmos.DrawSphere(transform.position, 0.25f);
        }        
        else
        {
            Gizmos.DrawWireSphere(transform.position, 0.25f);
        }
    }

    private void Update()
    {
        Value = IsPointOnScreen(transform.position) ? 1f : 0f;
    }

    private bool IsPointOnScreen(Vector3 point)
    {
        Vector3 screenPoint = _mainCamera.WorldToViewportPoint(point);
        return screenPoint.x >= screenMin.x && screenPoint.x <= screenMax.x &&
               screenPoint.y >= screenMin.y && screenPoint.y <= screenMax.y &&
               screenPoint.z > 0f;
    }
}
