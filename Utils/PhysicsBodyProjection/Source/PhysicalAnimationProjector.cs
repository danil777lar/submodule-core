using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Utils.PhysicsBodyProjections
{
    public class PhysicalAnimationProjector : MonoBehaviour
    {
        [SerializeField] private Transform _animationBody;
        [SerializeField] private ConfigurableJoint _connectionJointPrefab;

        public Transform AnimationBody => _animationBody;
        public ConfigurableJoint JointPrefab => _connectionJointPrefab;
    }
}
