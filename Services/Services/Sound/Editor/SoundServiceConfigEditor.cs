using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProjectConstants;

namespace Larje.Core.Services
{
    [CustomEditor(typeof(SoundServiceConfig))]
    public class SoundServiceConfigEditor : Editor
    {
        private ReorderableList _list;
        private Dictionary<string, ReorderableList> _subLists = new Dictionary<string, ReorderableList>();


        private void OnEnable()
        {
            _list = new ReorderableList(serializedObject, serializedObject.FindProperty("soundPacks"), false, false, false, false);

            float elementHeight = 0f;
            _list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var soundPack = _list.serializedProperty.GetArrayElementAtIndex(index);
                SerializedProperty soundType = soundPack.FindPropertyRelative("soundType");
                SerializedProperty sounds = soundPack.FindPropertyRelative("sounds");

                ReorderableList subList;
                string listKey = soundPack.propertyPath;
                if (_subLists.ContainsKey(listKey))
                    subList = _subLists[listKey];
                else
                {
                    subList = new ReorderableList(soundPack.serializedObject, sounds, true, true, true, true);
                    subList.drawElementCallback = (Rect subrect, int subindex, bool subIsActive, bool subIsFocused) =>
                    {
                        elementHeight = subrect.height;
                        SerializedProperty clip = sounds.GetArrayElementAtIndex(subindex).FindPropertyRelative("clip");
                        SerializedProperty volume = sounds.GetArrayElementAtIndex(subindex).FindPropertyRelative("volume");

                        EditorGUI.PropertyField(
                            new Rect(subrect.x, subrect.y, subrect.width / 2, EditorGUIUtility.singleLineHeight),
                            clip, GUIContent.none);

                        volume.floatValue = EditorGUI.Slider(
                            new Rect(subrect.x + (subrect.width / 2f) + 10, subrect.y, (subrect.width / 2f) - 10, EditorGUIUtility.singleLineHeight),
                            volume.floatValue, 0f, 1f);
                    };
                    subList.drawHeaderCallback = (Rect subRect) =>
                    {
                        string typeName = Enum.GetName(typeof(ProjectConstants.SoundType), (ProjectConstants.SoundType)soundType.enumValueIndex);
                        EditorGUI.LabelField(new Rect(subRect.x, subRect.y, subRect.width, subRect.height), typeName);
                    };
                    _subLists[listKey] = subList;
                }
                subList.DoList(new Rect(rect.x, rect.y, rect.width, rect.height));
            };
            _list.elementHeightCallback = (int index) =>
            {
                var element = _list.serializedProperty.GetArrayElementAtIndex(index);
                SerializedProperty sounds = element.FindPropertyRelative("sounds");
                int elementCount = Mathf.Max(1, sounds.arraySize); 
                return (elementCount * elementHeight) + (EditorGUIUtility.singleLineHeight * 3f);
            };
            _list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Sound Settings");
            };
        }

        public override void OnInspectorGUI()
        {
            List<SoundService.SoundPack> soundPacks = ((SoundServiceConfig)target).soundPacks;
            for (int i = 0; i < Enum.GetValues(typeof(SoundType)).Length; i++)
            {
                if (soundPacks.Count == i)
                {
                    SoundService.SoundPack soundPack = new SoundService.SoundPack();
                    soundPack.soundType = (SoundType)Enum.GetValues(typeof(SoundType)).GetValue(i);
                    soundPack.sounds = new List<SoundService.SoundOption>();
                    soundPacks.Add(soundPack);
                }
            }
            
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            _list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}