using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Larje.Core.Services.LevelManagement
{
    [CustomEditor(typeof(LevelManagerService))]
    public class LevelServiceEditor : Editor
    {
        private ReorderableList _list;

        private void OnEnable()
        {
            _list = new ReorderableList(serializedObject, serializedObject.FindProperty("_levelKeys"), true, false, true, true);
            _list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = _list.serializedProperty.GetArrayElementAtIndex(index);

                rect.y += 2;

                float buttonWidth = 50;
                Rect fieldRect = new Rect(rect.x, rect.y, rect.width - buttonWidth - 2, EditorGUIUtility.singleLineHeight);
                Rect buttonRect = new Rect(rect.x + rect.width - buttonWidth, rect.y, buttonWidth, EditorGUIUtility.singleLineHeight);
                element.stringValue = EditorGUI.TextField(fieldRect, element.stringValue);
                if (GUI.Button(buttonRect, "Spawn"))
                {
                    (target as LevelManagerService).SpawnLevelInDebugMode(_list.serializedProperty.GetArrayElementAtIndex(index).stringValue);
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}