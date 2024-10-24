using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Larje.Core.Entities
{
    public class Entity : MonoBehaviour
    {
        [field: SerializeField] public EntityId Id { get; private set; }
        
        private Dictionary<Type, List<Component>> _components = new Dictionary<Type, List<Component>>();

        private void Awake()
        {
            DIContainer.BindEntity(this);
        }

        private void OnDestroy()
        {
            DIContainer.UnbindEntity(this);
        }

        public bool TryFindComponents<T>(out IReadOnlyCollection<T> components) where T : Component
        {
            if (_components.TryGetValue(typeof(T), out List<Component> storedValue) && storedValue is { Count: > 0 })
            {
                components = storedValue.Cast<T>().ToList();
            }
            else
            {
                components = GetComponentsInChildren<T>(true).ToList();
                
                if (_components.ContainsKey(typeof(T)))
                {
                    _components[typeof(T)] = components.Cast<Component>().ToList();
                }
                else
                {
                    _components.Add(typeof(T), components.Cast<Component>().ToList());
                }
            }

            return components is { Count: > 0 };
        }
        
        public bool TryFindComponent<T>(out T component) where T : Component
        {
            bool result = TryFindComponents(out IReadOnlyCollection<T> components) && components.Count > 0;
            component = result ? components.First() : null;
            return result;
        }
    }
}