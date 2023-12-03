using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Larje.Core.Tools.GunController
{
    [RequireComponent(typeof(LineRenderer))]
    public class LaserSight : MonoBehaviour
    {
        [SerializeField] private LayerMask mask;
        [SerializeField] private float fullLength;
        [Header("Animation")] 
        [SerializeField] private bool animate;
        [SerializeField] private bool independentUpdate;
        [SerializeField] private float animationDuration;
        [SerializeField] private Ease animationEase;

        private float _length;
        private LineRenderer _line;
        
        private void Start()
        {
            _line = GetComponent<LineRenderer>();
            _length = animate ? 0f : fullLength;
            if (animate)
            {
                DOTween.To(() => 0f, (x) => _length = x, fullLength, animationDuration)
                    .SetUpdate(UpdateType.Normal, independentUpdate)
                    .SetEase(animationEase);
            }
        }

        private void Update()
        {
            DrawLaser();
        }

        private void DrawLaser()
        {
            List<Vector3> points = new List<Vector3>();
            points.Add(transform.position);
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _length, mask))
            {
                points.Add(hit.point);
            }
            else
            {
                points.Add(transform.position + transform.forward * _length);
            }
            _line.SetPositions(points.ToArray());
        }
    }
}