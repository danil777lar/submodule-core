using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public abstract class ButtonInteractionEffect
{
    public const string KEY_IS_ACTIVE = "isActive";
    public const string KEY_DELTA_TIME = "deltaTime";
    
    [SerializeField] private float speedIn = 5f;
    [SerializeField] private float speedOut = 5f;
    
    public float SpeedIn => speedIn;
    public float SpeedOut => speedOut;
    
    public abstract void Evaluate(ButtonInteractionConnector connector, Dictionary<string, string> data);

    public virtual void DrawEditor()
    {
        #if UNITY_EDITOR
        speedIn = UnityEditor.EditorGUILayout.FloatField("Speed In", speedIn);
        speedOut = UnityEditor.EditorGUILayout.FloatField("Speed Out", speedOut);
        #endif
    }

    protected float GetMovedValue(string key, Dictionary<string, string> data)
    {
        bool isActive = data.TryGetValue(KEY_IS_ACTIVE, out string activeValue) && bool.Parse(activeValue);
        float deltaTime = data.TryGetValue(KEY_DELTA_TIME, out string deltaTimeValue) ? float.Parse(deltaTimeValue) : 0f;
        
        float t = data.TryGetValue(key, out string tValue) ? float.Parse(tValue) : 0f;
        t += deltaTime * (isActive ? 1f : -1f) * (isActive ? SpeedIn : SpeedOut);
        t = Mathf.Clamp01(t);
        data[key] = t.ToString();

        return t;
    }
}
