using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools.CompositeProperties
{
    [Serializable]
    public class BoolComposite
    {
        [SerializeField] private bool defaultValue = true;
        [SerializeField] private Operation operation = Operation.And;
        [SerializeField] private bool value; 
        
        private List<Func<bool>> _values = new List<Func<bool>>();
        
        public bool Value
        {
            get
            {
                bool result = defaultValue;
                foreach (Func<bool> value in _values)
                {
                    if (operation == Operation.And)
                    {
                        result &= value();
                    }
                    else if (operation == Operation.Or)
                    {
                        result |= value();
                    }
                }

                value = result;
                return result;
            }
        }
        
        public void AddValue(Func<bool> value)
        {
            if (!_values.Contains(value))
            {
                _values.Add(value);
            }
        }

        public void RemoveValue(Func<bool> value)
        {
            if (_values.Contains(value))
            {
                _values.Remove(value);
            }
        }
        
        public enum Operation
        {
            And,
            Or
        }
    }
}