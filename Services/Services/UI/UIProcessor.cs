using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    public abstract class UIProcessor : MonoBehaviour
    {
        [field: SerializeField] public int Priority { get; private set; }
        
        protected Transform holder;
        
        public virtual void Init()
        {
            holder = new GameObject($"{GetType().Name} Holder").transform;
            holder.SetParent(transform);
            holder.localPosition = Vector3.zero;
            holder.localScale = Vector3.one;
        }
        
        public abstract bool ComputeDeviceBackButton();
    }
}