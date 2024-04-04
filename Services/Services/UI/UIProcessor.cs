using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    public abstract class UIProcessor : MonoBehaviour
    {
        [SerializeField] private int priority;
        
        protected Transform holder;
        
        public virtual void Init()
        {
            holder = new GameObject($"{GetType().Name} Holder").transform;
            holder.SetParent(transform);
            holder.localPosition = Vector3.zero;
            holder.localScale = Vector3.one;
        }
    }
}