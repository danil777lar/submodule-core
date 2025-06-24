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

        public bool TryGetValue(out T value)
        {
            Func<T> input = null;
            
            if (_values.Count > 0)
            {
                input = _values.OrderByDescending(x => x.Value.Invoke())
                    .First().Key;
            }

            if (input != null)
            {
                value = input.Invoke();
                return true;
            }
         
            value = default;
            return false;
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