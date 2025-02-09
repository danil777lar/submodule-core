using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Larje.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[BindService(typeof(InputService))]
public abstract class InputService : Service
{
    public abstract InputAction UIBack { get; }
    public abstract InputAction PlayerRun { get; }

    private List<Map> _maps = new List<Map>();
    
    public override void Init()
    {
        InputSystem_Actions input = new InputSystem_Actions();
        Type outerType = typeof(InputSystem_Actions);
        List<Type> nestedTypes = outerType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
            .ToList().FindAll(x => x.IsStruct());

        foreach (Type type in nestedTypes)
        {
            Map map = new Map();
            map.type = type;
            map.instance = Activator.CreateInstance(type, new object[]{input});
            map.conditions = new List<Func<bool>>();
            
            MethodInfo getMethod = type.GetMethod("Get");
            if (getMethod == null)
            {
                Debug.LogError("No Get method found for type " + type.Name);
                continue;
            }
            else
            {
                map.map = getMethod.Invoke(map.instance, null) as InputActionMap;   
            }
            
            _maps.Add(map);
        }
    }
    
    public T GetActions<T>()
    {
        foreach (Map map in _maps)
        {
            if (map.type == typeof(T))
            {
                return (T)map.instance;
            }
        }
        
        return default;
    }

    protected virtual void Update()
    {
        foreach (Map map in _maps)
        {
            bool isEnabled = true;
            foreach (Func<bool> condition in map.conditions)
            {
                isEnabled &= condition();
            }

            if (isEnabled)
            {
                map.map.Enable();
            }
            else
            {
                map.map.Disable();
            }
        }
    }

    private class Map
    {
        public InputActionMap map;
        public Type type;
        public object instance;
        public List<Func<bool>> conditions;
    }
}
