using System;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace Larje.Core.Services.DebugConsole
{
    public class DebugConsoleMethodPanelFieldInput : DebugConsoleMethodPanelField
    {
        [SerializeField] private TMP_InputField inputField;

        private Type _targetType;
        private Action<object> _onValueSet;

        public override void Init(ParameterInfo param, Action<object> onValueSet)
        {
            base.Init(param, onValueSet);

            _targetType = param.ParameterType;
            _onValueSet = onValueSet;
            inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
        }

        private void OnInputFieldValueChanged(string value)
        {
            object v = value;
            if (_targetType != typeof(string))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    v = Activator.CreateInstance(_targetType);
                }
                else
                {
                    try
                    {
                        v = Convert.ChangeType(value, _targetType);
                    }
                    catch (Exception e)
                    {
                        v = Activator.CreateInstance(_targetType);
                    }
                }
            }

            _onValueSet.Invoke(v);
        }
    }
}