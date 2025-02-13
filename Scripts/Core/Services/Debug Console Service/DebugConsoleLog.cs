using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DebugConsoleLog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private List<LogColor> logColors;
    
    public void Init(DebugConsoleService.Log log, int fontSize)
    {
        text.fontSize = fontSize;
        text.color = GetColor(log.type);
        text.text = $"[{log.type.ToString()}] {log.text}";
    }

    private void OnValidate()
    {
        logColors.ForEach(x => x.Validate());
    }

    private Color GetColor(LogType type)
    {
        Color color = Color.white;

        LogColor logColor = logColors.Find(x => x.Type == type);
        if (logColor != null)
        {
            color = logColor.Color;
        }

        return color;
    }

    [Serializable]
    private class LogColor
    {
        [SerializeField, HideInInspector] public string inspectorName; 
        [field: SerializeField] public LogType Type { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }

        public void Validate()
        {
            inspectorName = Type.ToString();
        }
    } 
}
