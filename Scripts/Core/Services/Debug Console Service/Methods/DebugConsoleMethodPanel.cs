using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DebugConsoleMethodPanel : MonoBehaviour
{
    [SerializeField] private Button executeButton;
    [SerializeField] private TextMeshProUGUI methodName;
    [Space] 
    [SerializeField] private DebugConsoleMethodPanelField panelFieldInput; 
    [SerializeField] private DebugConsoleMethodPanelField panelFieldDropDown;
    [SerializeField] private DebugConsoleMethodPanelField panelFieldObject;

    private MethodInfo _methodInfo;
    private Dictionary<ParameterInfo, object> _parameters;
    
    public void Init(MethodInfo methodInfo)
    {
        _methodInfo = methodInfo;
        
        methodName.text = methodInfo.Name;
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
        foreach (KeyValuePair<ParameterInfo, object> p in _parameters)
        {
            ParameterInfo parameter = p.Key;
            
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
            
            string label = $"{parameter.ParameterType.Name}: {parameter.Name}";
            Instantiate(field, transform).Init(parameter.GetType(), label,
                (v) => _parameters[parameter] = v);
        }
    }

    private void OnExecuteButtonClicked()
    {
        _methodInfo.Invoke(null, _parameters.Values.ToArray());
    }
}
