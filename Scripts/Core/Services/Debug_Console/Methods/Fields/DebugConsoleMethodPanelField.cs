using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Larje.Core.Services.DebugConsole
{
    public abstract class DebugConsoleMethodPanelField : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

        public virtual void Init(ParameterInfo param, Action<object> onValueSet)
        {
            label.text = GetLabel(param);
        }

        private string GetLabel(ParameterInfo param)
        {
            string typeColor = ColorUtility.ToHtmlStringRGB(label.color * 0.75f);
            
            string nameStr = param.Name;
            string typeStrRaw = param.ParameterType.Name.ToLower();
            if (typeStrRaw.Length > 7)
            {
                typeStrRaw = typeStrRaw.Substring(0, 7);
                typeStrRaw += "...";
            }
            
            string typeStr = $"<color=#{typeColor}>({typeStrRaw})</color>";
            return $"{typeStr} {nameStr}";
        }
    }
}