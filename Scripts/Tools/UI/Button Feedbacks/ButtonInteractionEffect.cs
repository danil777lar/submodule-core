using System;
using UnityEngine;

[Serializable]
public abstract class ButtonInteractionEffect
{
    public abstract void Evaluate(ButtonInteractionConnector connector, float t);

    public virtual void DrawEditor()
    {
        #if UNITY_EDITOR
        
        GUILayout.Label(GetType().Name, UnityEditor.EditorStyles.boldLabel);
        GUILayout.Space(10);
        #endif
    }
}
