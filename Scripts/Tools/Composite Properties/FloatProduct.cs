using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools.CompositeProperties
{
    [Serializable]
    public class FloatProduct
    {
        private List<Func<float>> _values = new List<Func<float>>();

        public float Value
        {
            get
            {
                float prod = 1f;
                foreach (Func<float> value in _values)
                {
                    prod *= value();
                }

                return prod;
            }
        }

        public void AddValue(Func<float> value)
        {
            if (!_values.Contains(value))
            {
                _values.Add(value);
            }
        }

        public void RemoveValue(Func<float> value)
        {
            if (_values.Contains(value))
            {
                _values.Remove(value);
            }
        }
    }
}