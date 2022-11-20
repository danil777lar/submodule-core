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
                _projector.BuildPhysicsBody();
            }
        }
    }
}