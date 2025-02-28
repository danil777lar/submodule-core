using System;
using System.Reflection;
using UnityEngine;

namespace Larje.Core.Services.DebugConsole
{
    public class DebugConsoleMethodPanelFieldObject : DebugConsoleMethodPanelField
    {
        public override void Init(ParameterInfo param, Action<object> onValueSet)
        {
            base.Init(param, onValueSet);
        }
    }
}