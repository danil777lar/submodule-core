using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Larje.Core.Tools.PhysicsBodyProjections
{
    [CustomEditor(typeof(PhysicalAnimationProjector))]
    public class PhysicalAnimationProjectorEditor : Editor
    {
        private PhysicalAnimationProjector _projector;


        private void OnEnable()
        {
            _projector = target as PhysicalAnimationProjector;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            if (GUILayout.Button("Build Physics Body"))
            {
                BuildPhysicsBody();
            }
        }


        private void BuildPhysicsBody()
        {
            List<Rigidbody> projectorBones = new List<Rigidbody>(_projector.GetComponentsInChildren<Rigidbody>());
            List<Rigidbody> animationBones = new List<Rigidbody>(_projector.AnimationBody.GetComponentsInChildren<Rigidbody>());

            foreach (Rigidbody bone in projectorBones)
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
                            propertyInfo.SetValue(newJoint, propertyInfo.GetValue(_projector.JointPrefab));
                        }
                        catch (Exception ex) { }
                    }
                    newJoint.connectedBody = animBone;
                }
            }
        }
    }
}