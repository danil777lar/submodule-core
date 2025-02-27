using System;
using TMPro;
using UnityEngine;

public class DebugConsoleMethodPanelFieldInput : DebugConsoleMethodPanelField
{
    [SerializeField] private TMP_InputField inputField;
    
    private Action<object> _onValueSet;
    
    public override void Init(Type type, string paramName, Action<object> onValueSet)
    {
        base.Init(type, paramName, onValueSet);
        
        _onValueSet = onValueSet;
        inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
    }
    
    private void OnInputFieldValueChanged(string value)
    {
        _onValueSet.Invoke(inputField.text);
    }
}
