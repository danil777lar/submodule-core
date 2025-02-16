using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class DebugConsoleLog : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private List<LogColor> logColors;

    private bool _drawStack;
    private DebugConsoleService.Log _log;
    
    public void Init(DebugConsoleService.Log log, int fontSize)
    {
        _log = log;
        text.fontSize = fontSize;
        DrawText();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        _drawStack = !_drawStack;
        DrawText();   
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

    private void DrawText()
    {
        text.color = GetColor(_log.type);
        text.text = $"[{_log.type.ToString()}] {_log.text}";
        if (_drawStack)
        {
            text.text += $"\n\nStack trace:\n{_log.stackTrace}";
        }
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
