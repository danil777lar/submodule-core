using System.Collections.Generic;
using Larje.Core.Tools;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ButtonInteractionFeedbackConfig))]
public class ButtonInteractionFeedbackConfigEditor : Editor
{
    private Dictionary<ButtonInteractionState, bool> _stateFoldouts = new Dictionary<ButtonInteractionState, bool>();

    public override void OnInspectorGUI()
    {
        ButtonInteractionFeedbackConfig config = (ButtonInteractionFeedbackConfig)target;
        
        foreach (ButtonInteractionState state in config.States)
        {
            DrawState(state);
        }
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
            state.durationIn = EditorGUILayout.FloatField("Duration In", state.durationIn);
            state.durationOut = EditorGUILayout.FloatField("Duration Out", state.durationOut);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            
            GUILayout.BeginVertical(EditorStyles.helpBox);
            
            GUILayout.BeginHorizontal();
            
            GUILayout.Label($"Effects ");
            if (GUILayout.Button("Add Effect"))
            {
                AddEffect(state);
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

    private void AddEffect(ButtonInteractionState state)
    {
        state.Effects.Add(new ScaleEffect());
    }
}
