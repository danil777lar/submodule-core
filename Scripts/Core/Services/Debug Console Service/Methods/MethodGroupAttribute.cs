using System;using UnityEngine;

namespace Larje.Core.Services.DebugConsole
{
    public class MethodGroupAttribute : Attribute
    {
        public string GroupName { get; private set; }

        public MethodGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}