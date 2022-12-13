using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Larje.Core.Services
{
    [CustomEditor(typeof(DataService))]
    public class DataServiceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                base.OnInspectorGUI();
            }
            else 
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_profileName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultProfile"));
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}