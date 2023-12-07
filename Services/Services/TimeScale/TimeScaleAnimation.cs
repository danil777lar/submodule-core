using System;
using MoreMountains.Tools;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    [Serializable]
    public class TimeScaleAnimation
    {
        [SerializeField, HideInInspector] private string inspectorName;
        
        [field: SerializeField] public TimeScaleAnimationType Type;
        [field: SerializeField] public TimeScaleLayerType Layer;
        [field: SerializeField] public float Duration = 1f;
        [field: SerializeField, MMVector("zero", "one")] public Vector2 RemapValues = new Vector2(0f, 1f);
        [field: SerializeField] public AnimationCurve Curve;

        public void Validate()
        {
            inspectorName = Type.ToString();
        }
    }
}