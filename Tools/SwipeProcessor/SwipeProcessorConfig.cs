using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools.SwipeProcessor
{
    [CreateAssetMenu(fileName = "Swipes Config", menuName = "Configs/Swipes Config")]
    public class SwipeProcessorConfig : ScriptableObject
    {
        [field: SerializeField] public bool Enabled = true;
        [field: SerializeField, Range(0f, 1f)] public float MinLength = 0.1f;
        [field: SerializeField, Range(0f, 1f)] public float MaxLength = 0.4f;
        [field: SerializeField, Range(0f, 180f)] public float MaxAngle = 45f;
    }
}