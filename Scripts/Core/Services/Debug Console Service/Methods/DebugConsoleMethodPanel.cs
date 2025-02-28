using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Larje.Core.Services.DebugConsole
{
    public class DebugConsoleMethodPanel : MonoBehaviour
    {
        [SerializeField] private Button executeButton;
        [SerializeField] private TextMeshProUGUI methodName;
        [SerializeField] private Transform content;
        [Space] [SerializeField] private DebugConsoleMethodPanelField panelFieldInput;
        [SerializeField] private DebugConsoleMethodPanelField panelFieldDropDown;
        [SerializeField] private DebugConsoleMethodPanelField panelFieldObject;

        private MethodInfo _methodInfo;
        private Dictionary<ParameterInfo, object> _parameters;

        public MethodInfo MethodInfo => _methodInfo;

        public void Init(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;

            methodName.text = FormatName(methodInfo.Name);
            executeButton.onClick.AddListener(OnExecuteButtonClicked);

            _parameters = new Dictionary<ParameterInfo, object>();
            foreach (ParameterInfo parameter in _methodInfo.GetParameters())
            {
                _parameters.Add(parameter, null);
            }

            SpawnParameterFields();
        }

        private void SpawnParameterFields()
        {
            List<ParameterInfo> parameters = _parameters.Keys.ToList();
            foreach (ParameterInfo parameter in parameters)
            {
                DebugConsoleMethodPanelField field = null;
                if (parameter.ParameterType.IsPrimitive || parameter.ParameterType == typeof(string))
                {
                    field = panelFieldInput;
                }
                else if (parameter.ParameterType.IsEnum)
                {
                    field = panelFieldDropDown;
                }
                else
                {
                    field = panelFieldObject;
                }
                
                Instantiate(field, content).Init(parameter, (v) => _parameters[parameter] = v);
            }
        }

        private void OnExecuteButtonClicked()
        {
            try
            {
                _methodInfo.Invoke(null, _parameters.Values.ToArray());
            }
            catch (TargetInvocationException e)
            {
                Debug.LogWarning(e.InnerException.Message);
            }
        }

        private string FormatName(string input)
        {
            string result = input;
            for (int i = 1; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]))
                {
                    result = result.Insert(i, " ");
                    i++;
                }
            }
            return result;
        }
    }
}