using System;
using System.Linq;
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
        [SerializeField] private List<BodyPartJointSettings> _jointSettings;
        [Header("Debug")]
        [SerializeField] private bool _drawBonesDebug;
        [SerializeField] private bool _drawIfSelected;
        [SerializeField] private float _spheresRadius = 0.1f;
        [SerializeField] private Color _animationBodyColor = Color.red;
        [SerializeField] private Color _physicalBodyColor = Color.blue;

        private Dictionary<Transform, Transform> _bonesWithoutJoints;

        public Transform AnimationBody => _animationBody;
        public Transform PhysicalBody => _physicalBody;
        public ConfigurableJoint JointPrefab => _connectionJointPrefab;


        private void Start()
        {
            _bonesWithoutJoints = new Dictionary<Transform, Transform>();
            List<Transform> animationBones = AnimationBody.GetComponentsInChildren<Transform>().Where(x => x.GetComponent<Joint>() == null).ToList();
            List<Transform> physicalBones = PhysicalBody.GetComponentsInChildren<Transform>().Where(x => x.GetComponent<Joint>() == null).ToList();
            foreach (Transform physBone in physicalBones) 
            {
                _bonesWithoutJoints.Add(physBone, animationBones.Find(x => x.gameObject.name == physBone.gameObject.name));
            }
        }

        private void FixedUpdate()
        {
            if (_jointSettings != null)
            {
                foreach (BodyPartJointSettings settings in _jointSettings)
                {
                    settings.UpdateSettings();
                }
            }

            PhysicalBody.localPosition = AnimationBody.localPosition;
            PhysicalBody.localRotation = AnimationBody.localRotation;
            foreach (Transform physBone in _bonesWithoutJoints.Keys) 
            {
                Transform animBone = _bonesWithoutJoints[physBone];
                if (animBone != null)
                {
                    physBone.localPosition = animBone.localPosition;
                    physBone.localRotation = animBone.localRotation;
                }
            }
        }

        private void OnValidate()
        {
            if (_jointSettings != null) 
            {
                foreach (BodyPartJointSettings settings in _jointSettings) 
                {
                    settings.UpdateInspectorName();
                }
            }
        }

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
                        catch (Exception ex)
                        {
                            Debug.LogWarning(ex);
                        }
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


        public enum BodyPartType 
        {
            Hips,
            Spine,
            Head,
            LeftArm, 
            RightArm, 
            LeftLeg,
            RightLeg
        }

        [Serializable]
        public class BodyPartJointSettings
        {
            [SerializeField, HideInInspector] private string _inspectorName;

            public BodyPartType partType;
            public Settings settings;

            [Space]
            [SerializeField] private List<ConfigurableJoint> _joints;


            public void UpdateInspectorName() 
            {
                _inspectorName = Enum.GetName(typeof(BodyPartType), partType);
            }

            public void UpdateSettings() 
            {
                foreach (ConfigurableJoint joint in _joints) 
                {
                    JointDrive drive = new JointDrive();
                    drive.positionSpring = settings.positionSpring;
                    drive.positionDamper = settings.positionDamper;
                    drive.maximumForce = settings.maximumForce;

                    joint.slerpDrive = drive;
                    joint.massScale = settings.massScale;

                    joint.xMotion = settings.lockPosition ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;
                    joint.yMotion = settings.lockPosition ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;
                    joint.zMotion = settings.lockPosition ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;

                    joint.angularXMotion = settings.lockRotation ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;
                    joint.angularYMotion = settings.lockRotation ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;
                    joint.angularZMotion = settings.lockRotation ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;
                }
            }


            [Serializable]
            public struct Settings 
            {
                public bool lockPosition;
                public bool lockRotation;
                public float positionSpring;
                public float positionDamper;
                public float maximumForce;
                public float massScale;
            }
        }
    }
}
