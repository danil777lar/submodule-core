using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Larje.Core.Services.DebugConsole
{
    public class DebugConsoleMethodPanelFieldToggle : DebugConsoleMethodPanelField
    {
        [SerializeField] private Toggle toggle;

        public override void Init(ParameterInfo param, Action<object> onValueSet)
        {
            base.Init(param, onValueSet);

            toggle.isOn = false;
            onValueSet.Invoke(false);
            toggle.onValueChanged.AddListener(value => onValueSet.Invoke(value));
        }
    }
}
