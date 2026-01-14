using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace Larje.Core.Services.DebugConsole
{
    public class DebugConsoleMethodPanelFieldDropDown : DebugConsoleMethodPanelField
    {
        [SerializeField] private TMP_Dropdown dropdown;

        private Type _targetType;
        private Dictionary<string, object> _values;
        private Action<object> _onValueSet;

        public override void Init(ParameterInfo param, Action<object> onValueSet)
        {
            base.Init(param, onValueSet);

            _targetType = param.ParameterType;
            _onValueSet = onValueSet;

            InitDropDown();
            dropdown.onValueChanged.AddListener(OnDropDownValueChanged);
        }

        private void InitDropDown()
        {
            _values = new Dictionary<string, object>();

            dropdown.ClearOptions();
            if (_targetType.IsEnum)
            {
                string[] names = Enum.GetNames(_targetType);
                foreach (string n in names)
                {
                    _values.Add(n, Enum.Parse(_targetType, n));
                }

                dropdown.AddOptions(_values.Keys.ToList());
                dropdown.value = 0;
                OnDropDownValueChanged(0);
            }
        }

        private void OnDropDownValueChanged(int value)
        {
            if (value < 0 || value >= dropdown.options.Count)
            {
                return;
            }

            string key = dropdown.options[value].text;
            _onValueSet.Invoke(_values[key]);
        }
    }
}