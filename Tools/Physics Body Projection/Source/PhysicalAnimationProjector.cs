using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools.PhysicsBodyProjections
{
    public class PhysicalAnimationProjector : MonoBehaviour
    {
        [SerializeField] private Transform _animationBody;
        [SerializeField] private Transform _physicalBody;
        [SerializeField] private ConfigurableJoint _connectionJointPrefab;
        [Header("Debug")]
        [SerializeField] private bool _drawBonesDebug;
        [SerializeField] private bool _drawIfSelected;
        [SerializeField] private float _spheresRadius = 0.1f;
        [SerializeField] private Color _animationBodyColor = Color.red;
        [SerializeField] private Color _physicalBodyColor = Color.blue;


        public Transform AnimationBody => _animationBody;
        public Transform PhysicalBody => _physicalBody;
        public ConfigurableJoint JointPrefab => _connectionJointPrefab;


        public void BuildPhysicsBody()
        {
            List<Rigidbody> physicalBones = new List<Rigidbody>(PhysicalBody.GetComponentsInChildren<Rigidbody>());
            List<Rigidbody> animationBones = new List<Rigidbody>(AnimationBody.GetComponentsInChildren<Rigidbody>());

            foreach (Rigidbody bone in physicalBones)
            {
                Rigidbody animBone = animationBones.Find((ab) => ab.name == bone.name);
                if (animBone)
                {
                    animBone.isKinematic = true;
                    foreach (Collider collider in animBone.GetComponents<Collider>())
                        DestroyImmediate(collider);
                    foreach (Joint joint in animBone.GetComponents<Joint>())
                        DestroyImmediate(joint);
                    foreach (ConfigurableJoint joint in bone.GetComponents<ConfigurableJoint>())
                        DestroyImmediate(joint);

                    CharacterJoint characterJoint = bone.GetComponent<CharacterJoint>();
                    if (characterJoint)
                    {
                        characterJoint.twistLimitSpring = new SoftJointLimitSpring() { damper = 0f, spring = 1f };
                        characterJoint.lowTwistLimit = new SoftJointLimit() { bounciness = 1f, contactDistance = 0f, limit = characterJoint.lowTwistLimit.limit };
                        characterJoint.highTwistLimit = new SoftJointLimit() { bounciness = 1f, contactDistance = 0f, limit = characterJoint.highTwistLimit.limit };
                        characterJoint.swingLimitSpring = new SoftJointLimitSpring() { damper = 0f, spring = 1f };
                        characterJoint.swing1Limit = new SoftJointLimit() { bounciness = 1f, contactDistance = 0f, limit = characterJoint.swing1Limit.limit };
                        characterJoint.swing2Limit = new SoftJointLimit() { bounciness = 1f, contactDistance = 0f, limit = characterJoint.swing2Limit.limit };
                    }

                    ConfigurableJoint newJoint = bone.gameObject.AddComponent<ConfigurableJoint>();
                    foreach (PropertyInfo propertyInfo in typeof(ConfigurableJoint).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                    {
                        try
                        {
                            propertyInfo.SetValue(newJoint, propertyInfo.GetValue(JointPrefab));
                        }
                        catch (Exception ex) { }
                    }
                    newJoint.connectedBody = animBone;
                }
            }
        }

        #region Debug

        private void OnDrawGizmos()
        {
            if (_drawBonesDebug && !_drawIfSelected)
            {
                DrawDebugBody();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_drawBonesDebug && _drawIfSelected)
            {
                DrawDebugBody();
            }
        }

        private void DrawDebugBody() 
        {
            DrawLineToChild(AnimationBody, _animationBodyColor, _spheresRadius);
            DrawLineToChild(PhysicalBody, _physicalBodyColor, _spheresRadius);
        }

        private void DrawLineToChild(Transform targetTransform, Color color, float sphereRadius) 
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(targetTransform.position, sphereRadius);
            foreach (Transform child in targetTransform) 
            {
                Gizmos.DrawLine(targetTransform.position, child.position);
                DrawLineToChild(child, color, sphereRadius);
            }
        }

        #endregion
    }
}
