using System;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class DebugConsoleMethodPanelField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    
    public virtual void Init(Type type, string paramName, Action<object> onValueSet)
    {
        label.text = paramName;
    }
}
