using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools.CompositeProperties
{
    [Serializable]
    public class Vector3Sum
    {
        private List<Func<Vector3>> _vector3List = new List<Func<Vector3>>();
        
        public Vector3 Value
        {
            get
            {
                Vector3 sum = Vector3.zero;
                foreach (var vector3 in _vector3List)
                {
                    sum += vector3();
                }
                return sum;
            }
        }

        public void AddValue(Func<Vector3> value)
        {
            if (!_vector3List.Contains(value))
            {
                _vector3List.Add(value);
            }
        }

        public void RemoveValue(Func<Vector3> value)
        {
            if (_vector3List.Contains(value))
            {
                _vector3List.Remove(value);
            }
        }
    }
}