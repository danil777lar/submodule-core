using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using UnityEngine;

namespace Larje.Core.Entities
{
    public class Entity : MonoBehaviour
    {
        [field: SerializeField] public EntityId Id { get; private set; }
        
        private Dictionary<Type, Component> _components = new Dictionary<Type, Component>();

        private void Awake()
        {
            DIContainer.BindEntity(this);
        }

        private void OnDestroy()
        {
            DIContainer.UnbindEntity(this);
        }

        public T FindComponent<T>()
        {
            return GetComponent<T>();
        }
    }
}