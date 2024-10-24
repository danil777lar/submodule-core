using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using UnityEngine;

namespace Larje.Core.Entities
{
    public class Entity : MonoBehaviour
    {
        [field: SerializeField] public EntityId Id { get; private set; }
        
        private Dictionary<Type, Component> _components;

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