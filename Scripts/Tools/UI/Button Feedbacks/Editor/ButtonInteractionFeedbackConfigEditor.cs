using System;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Tools;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ButtonInteractionFeedbackConfig))]
public class ButtonInteractionFeedbackConfigEditor : Editor
{
    private Type[] _effectTypes = Type.EmptyTypes;
    private Dictionary<ButtonInteractionState, int> _selectedEffects = new Dictionary<ButtonInteractionState, int>();
    private Dictionary<ButtonInteractionState, bool> _stateFoldouts = new Dictionary<ButtonInteractionState, bool>();

    public override void OnInspectorGUI()
    {
        ButtonInteractionFeedbackConfig config = (ButtonInteractionFeedbackConfig)target;
        
        SerializedProperty materialProp = serializedObject.FindProperty("material");
        EditorGUILayout.PropertyField(materialProp, new GUIContent("Material"));
        GUILayout.Space(20);
        
        foreach (ButtonInteractionState state in config.States)
        {
            DrawState(state);
        }
        
        GUILayout.Space(20);
        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }

    private void OnEnable()
    {
        _effectTypes = GetEffects();
    }

    private void DrawState(ButtonInteractionState state)
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);

        _stateFoldouts.TryAdd(state, false);
        string foldoutLabel = (_stateFoldouts[state] ? "▼ " : "► ") + state.stateType.ToString();
        if (GUILayout.Button(foldoutLabel, EditorStyles.boldLabel))
        {
            _stateFoldouts[state] = !_stateFoldouts[state];
        }

        //STATE CONTENT /////////
        if (_stateFoldouts[state])
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();
            
            GUILayout.Label($"Effects ");
            
            int selectedEffect = _selectedEffects.ContainsKey(state) ? _selectedEffects[state] : 0;
            string[] effectOptions = _effectTypes.Select((type) => type.ToString()).ToArray();
            _selectedEffects[state] = EditorGUILayout.Popup("", selectedEffect, effectOptions); 
            if (GUILayout.Button("Add"))
            {
                AddEffect(state, _selectedEffects[state]);
            }
            
            GUILayout.EndHorizontal();
            
            foreach (ButtonInteractionEffect effect in state.Effects)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                
                GUILayout.BeginHorizontal();
                GUILayout.Label(effect.GetType().Name, UnityEditor.EditorStyles.boldLabel);
                if (GUILayout.Button("X"))
                {
                    state.Effects.Remove(effect);
                    break;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                
                effect.DrawEditor();
                
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        ///////////////////////////
        
        GUILayout.EndVertical();
    }

    private void AddEffect(ButtonInteractionState state, int effectIndex = -1)
    {
        if (effectIndex >= 0)
        {
            state.Effects.Add(GetEffect(effectIndex));
        }
    }

    private ButtonInteractionEffect GetEffect(int index)
    {
        ButtonInteractionEffect effect = (ButtonInteractionEffect)Activator.CreateInstance(_effectTypes[index]);
        return effect;
    }
    
    private Type[] GetEffects()
    {
        Type[] effects = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(ButtonInteractionEffect).IsAssignableFrom(type) && !type.IsAbstract && type != typeof(ButtonInteractionEffect))
            .ToArray();

        return effects;
    }
}
