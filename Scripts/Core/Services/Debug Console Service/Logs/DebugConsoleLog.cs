using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Larje.Core.Services.DebugConsole
{
    public class DebugConsoleLog : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_InputField text;
        [SerializeField] private Image controlRect; 
        [SerializeField] private List<LogColor> logColors;

        private bool _drawStack;
        private DebugConsoleService.Log _log;
        private Action _onPointerClick;

        public void Init(DebugConsoleService.Log log, int fontSize, Action onPointerClick)
        {
            _log = log;
            text.textComponent.fontSize = fontSize;
            _onPointerClick = onPointerClick;

            DrawText();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _drawStack = !_drawStack;
            DrawText();

            _onPointerClick?.Invoke();
        }

        private void Update()
        {
            RectTransform rect = (RectTransform)text.transform;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, text.textComponent.preferredHeight);

            RectTransform rectChild = (RectTransform)text.textComponent.transform.parent;
            rectChild.sizeDelta = new Vector2(rectChild.sizeDelta.x, text.textComponent.preferredHeight);
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
            controlRect.color = GetColor(_log.type);
            text.textComponent.color = GetColor(_log.type);
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
}