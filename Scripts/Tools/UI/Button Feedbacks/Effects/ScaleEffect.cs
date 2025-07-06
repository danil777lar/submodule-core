using System;
using UnityEngine;

[Serializable]
public class ScaleEffect : ButtonInteractionEffect
{
    [SerializeField] private float remapZero = 0f;
    [SerializeField] private float remapOne = 1f;
    [Space]
    [SerializeField] private AnimationCurve xCurve = AnimationCurve.Linear(0, 1, 1, 1);
    [SerializeField] private AnimationCurve yCurve = AnimationCurve.Linear(0, 1, 1, 1);
    [SerializeField] private AnimationCurve zCurve = AnimationCurve.Linear(0, 1, 1, 1);
    
    public override void Evaluate(ButtonInteractionConnector connector, float t)
    {
        Vector3 value = Vector3.one;
        connector.GetPart<ConnectorScale>().Add(value);
    }

    public override void DrawEditor()
    {
        #if UNITY_EDITOR
        
        base.DrawEditor();
        
        remapZero = UnityEditor.EditorGUILayout.FloatField("Remap Zero", remapZero);
        remapOne = UnityEditor.EditorGUILayout.FloatField("Remap One", remapOne);
        xCurve = UnityEditor.EditorGUILayout.CurveField("X Curve", xCurve);
        yCurve = UnityEditor.EditorGUILayout.CurveField("Y Curve", yCurve);
        zCurve = UnityEditor.EditorGUILayout.CurveField("Z Curve", zCurve);
        
        #endif
    }
}
