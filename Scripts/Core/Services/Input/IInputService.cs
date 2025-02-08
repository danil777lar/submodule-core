using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dreamteck.Splines;
using Larje.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[BindService(typeof(InputService))]
public class InputService : Service
{
    private Dictionary<Type, object> _actions = new Dictionary<Type, object>();
    
    public override void Init()
    {
        Type outerType = typeof(InputSystem_Actions);
        List<Type> nestedTypes = outerType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
            .ToList().FindAll(x => x.IsStruct());

        foreach (Type type in nestedTypes)
        {
            object instance = Activator.CreateInstance(type);
            _actions.Add(type, instance);
        }
    }
    
    public T GetActions<T>()
    {
        return (T)_actions[typeof(T)];
    }
}
