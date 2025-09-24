using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScaleEffect : ButtonInteractionEffect
{
    private const string KEY_VALUE = "scale_effect_value";
    
    [SerializeField] private AnimationCurve xCurve = AnimationCurve.Linear(0, 1, 1, 1);
    [SerializeField] private AnimationCurve yCurve = AnimationCurve.Linear(0, 1, 1, 1);
    [SerializeField] private AnimationCurve zCurve = AnimationCurve.Linear(0, 1, 1, 1);
    
    public override void Evaluate(ButtonInteractionConnector connector, Dictionary<string, string> data)
    {
        float t = GetMovedValue(KEY_VALUE, data);
        Vector3 value = new Vector3(xCurve.Evaluate(t), yCurve.Evaluate(t), zCurve.Evaluate(t));
        connector.GetPart<ConnectorScale>().Add(value);
    }

    public override void DrawEditor()
    {
        #if UNITY_EDITOR
        
        base.DrawEditor();
        
        xCurve = UnityEditor.EditorGUILayout.CurveField("X Curve", xCurve);
        yCurve = UnityEditor.EditorGUILayout.CurveField("Y Curve", yCurve);
        zCurve = UnityEditor.EditorGUILayout.CurveField("Z Curve", zCurve);
        
        #endif
    }
}
