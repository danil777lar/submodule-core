using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MaterialFloatPropertyEffect : ButtonInteractionEffect
{
    private const string KEY_VALUE = "material_float_property_effect_value";
    
    [SerializeField] private string propertyName;
    [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 1, 1, 1);
    
    public override void Evaluate(ButtonInteractionConnector connector, Dictionary<string, string> data)
    {
        float t = GetMovedValue(KEY_VALUE, data);
        float value = curve.Evaluate(t);
        connector.GetPart<ConnectorMaterialFloatProperty>().Add(propertyName, value);
    }

    public override void DrawEditor()
    {
    #if UNITY_EDITOR
        
        base.DrawEditor();
        propertyName = UnityEditor.EditorGUILayout.TextField("Property Name", propertyName);
        curve = UnityEditor.EditorGUILayout.CurveField("Curve", curve);
        
    #endif
    }
}
