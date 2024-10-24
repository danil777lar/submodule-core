using System;
using UnityEngine;

namespace Larje.Core.Tools
{
    public class ConstDropdownAttribute : PropertyAttribute
    {
        public Type targetClass;

        public ConstDropdownAttribute(Type targetClass)
        {
            this.targetClass = targetClass;
        }
    }
}
