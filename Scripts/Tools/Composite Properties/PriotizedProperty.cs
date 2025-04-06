using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Larje.Core.Tools.CompositeProperties
{
    [Serializable]
    public class PriotizedProperty<T> 
    {
        private Dictionary<Func<T>, Func<int>> _values = new Dictionary<Func<T>, Func<int>>();
        
        public T Value
        {
            get
            {
                Func<T> input = _values.OrderByDescending(x => x.Value.Invoke())
                    .First().Key;
                
                return input != null ? input.Invoke() : default; 
            }
        }
        
        public void AddValue(Func<T> value, Func<int> priority)
        {
            if (!_values.ContainsKey(value))
            {
                _values.Add(value, priority);
            }
        }
        
        public void RemoveValue(Func<T> value)
        {
            if (_values.ContainsKey(value))
            {
                _values.Remove(value);
            }
        }
    }
}