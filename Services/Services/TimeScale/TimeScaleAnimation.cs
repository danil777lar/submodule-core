using System;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    [Serializable]
    public class TimeScaleAnimation
    {
        [field: SerializeField] public TimeScaleAnimationType Type;
        [field: SerializeField] public TimeScaleLayerType Layer;
        [field: SerializeField] public float Duration;
        [field: SerializeField] public AnimationCurve Curve;
    }
}