using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core
{
    [BindService(typeof(GameEventService))]
    public class GameEventService : Service
    {
        private Dictionary<Type, List<Delegate>> callbacks = new Dictionary<Type, List<Delegate>>();

        public override void Init()
        {
        }

        public void SendEvent(GameEvent gameEvent)
        {
            if (!gameEvent.IsValid)
            {
                return;
            }

            Type eventType = gameEvent.GetType();
            if (callbacks.TryGetValue(eventType, out List<Delegate> eventCallbacks))
            {
                Delegate[] callbacksCopy = eventCallbacks.ToArray();
                foreach (Delegate callback in callbacksCopy)
                {
                    callback.DynamicInvoke(gameEvent);
                }
            }
        }

        public void Subscribe<T>(Action<T> callback) where T : GameEvent
        {
            Type eventType = typeof(T);
            if (!callbacks.ContainsKey(eventType))
            {
                callbacks[eventType] = new List<Delegate>();
            }
            callbacks[eventType].Add(callback);
        }

        public void Unsubscribe<T>(Action<T> callback) where T : GameEvent
        {
            Type eventType = typeof(T);
            if (callbacks.TryGetValue(eventType, out var eventCallbacks))
            {
                eventCallbacks.Remove(callback);
                if (eventCallbacks.Count == 0)
                {
                    callbacks.Remove(eventType);
                }
            }
        }
    }
}
