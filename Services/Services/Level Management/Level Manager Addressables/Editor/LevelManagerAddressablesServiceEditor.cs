using System.Collections;
using System.Collections.Generic;
using Cinemachine.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Larje.Core.Services
{
    [CustomEditor(typeof(LevelManagerAddressablesService))]
    public class LevelManagerAddressablesServiceEditor : Editor
    {
        private ReorderableList _list;
        private float _buttonWidth = 50;
        private float _toggleWidth = EditorGUIUtility.singleLineHeight;

        private void OnEnable()
        {
            _list = new ReorderableList(serializedObject, serializedObject.FindProperty("levels"), true, false, true, true);
            _list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = _list.serializedProperty.GetArrayElementAtIndex(index);
                var key = element.FindPropertyRelative("key");
                var addToRandomList = element.FindPropertyRelative("addToRandomList");

                rect.y += 2;
                Rect fieldRect = new Rect(rect.x, rect.y, rect.width - _buttonWidth - _toggleWidth - 4, EditorGUIUtility.singleLineHeight);
                Rect toggleRect = new Rect(rect.x + rect.width - _buttonWidth - _toggleWidth, rect.y, _toggleWidth, EditorGUIUtility.singleLineHeight);
                Rect buttonRect = new Rect(rect.x + rect.width - _buttonWidth, rect.y, _buttonWidth, EditorGUIUtility.singleLineHeight);
                key.stringValue = EditorGUI.TextField(fieldRect, key.stringValue);
                addToRandomList.boolValue = EditorGUI.Toggle(toggleRect, addToRandomList.boolValue);
                if (GUI.Button(buttonRect, "Spawn"))
                {
                    (target as LevelManagerAddressablesService).SpawnLevelInDebugMode(key.stringValue);
                }
            };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            _list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}