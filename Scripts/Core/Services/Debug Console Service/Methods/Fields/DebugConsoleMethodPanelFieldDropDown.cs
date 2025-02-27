using System;
using UnityEngine;

public class DebugConsoleMethodPanelFieldDropDown : DebugConsoleMethodPanelField
{
    public override void Init(Type type, string paramName, Action<object> onValueSet)
    {
        base.Init(type, paramName, onValueSet);
    }
}
