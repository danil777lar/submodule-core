using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Larje.Core;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[BindService(typeof(InputService))]
public abstract class InputService : Service
{
    public abstract InputAction UIBack { get; }
    public abstract InputAction UIDebug { get; }
    
    public abstract Vector2 PlayerMovement { get; }
    public abstract InputAction PlayerRun { get; }
    public abstract InputAction PlayerPointer { get; }
    
    public abstract Dictionary<Type, bool> DefaultStates { get; }

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
            map.instance = Activator.CreateInstance(type, new object[] { input });
            map.conditions = new List<Condition>();

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
    
    public InputAction GetAction(string mapName, string actionName)
    {
        Map map = _maps.Find(map => map.type.Name == mapName);
        if (map != null)
        {
            InputAction action = map.map.FindAction(actionName);
            if (action != null)
            {
                return action;
            }
            else
            {
                Debug.LogError("Input Service | Cant find action " + actionName + " in map " + mapName);
            }
        }
        else
        {
            Debug.LogError("Input Service | Cant find map of type " + mapName);
        }

        
        return null;
    }

    public void AddCondition<T>(Condition condition)
    {
        Map map = _maps.Find(map => map.type == typeof(T));
        if (map != null && !map.conditions.Contains(condition))
        {
            map.conditions.Add(condition);
        }
    }

    public void RemoveCondition<T>(Condition condition)
    {
        Map map = _maps.Find(map => map.type == typeof(T));
        if (map != null && map.conditions.Contains(condition))
        {
            map.conditions.Remove(condition);
        }
    }

    protected virtual void Update()
    {
        foreach (Map map in _maps)
        {
            bool isEnabled = DefaultStates[map.type];
            string debug = $"Map " + map.type.Name + $" conditions: \n {isEnabled}";
            if (map.conditions.Count > 0)
            {
                List<Condition> orConditions = map.conditions
                    .FindAll(condition => condition.Operation == ConditionOperation.Or);
                List<Condition> andConditions = map.conditions
                    .FindAll(condition => condition.Operation == ConditionOperation.And);
                
                foreach (Condition condition in orConditions)
                {
                    isEnabled |= condition.Func();
                    debug += $" | {condition.Name}:{condition.Func()}";
                }
                
                foreach (Condition condition in andConditions)
                {
                    isEnabled &= condition.Func();
                    debug += $" & {condition.Name}:{condition.Func()}";
                }
                
                debug += $" = {isEnabled}";
                //MMDebug.DebugOnScreen(debug);
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
    
    public enum ConditionOperation
    {
        And,
        Or
    }
    
    public class Condition
    {
        public Func<bool> Func { get; }
        public ConditionOperation Operation { get; }
        public string Name { get; }
        
        public Condition(Func<bool> func, ConditionOperation operation = ConditionOperation.And, string name = "")
        {
            Func = func;
            Operation = operation;
            Name = name;
        }
    }
    
    private class Map
    {
        public InputActionMap map;
        public Type type;
        public object instance;
        public List<Condition> conditions;
    }
}
