using System;
using Larje.Core;
using UnityEngine;
using UnityEngine.Events;

public class TriggerReceiver : MonoBehaviour
{
    [SerializeField] private string key;
    [SerializeField] private TriggerConstant trigger;
    [Space]
    [Space] public UnityEvent<float> EventValueChangedFloatNormal;
    [Space] public UnityEvent<float> EventValueChangedFloatReversed;
    [Space]
    [Space] public UnityEvent<bool> EventValueChangedBoolNormal;
    [Space] public UnityEvent<bool> EventValueChangedBoolReversed;
    [Space] 
    [Space] public UnityEvent EventPositive;

    [InjectService] private GameEventService _gameEventService;
    
    public void OnValueChangedFloat(float value)
    {
        EventValueChangedFloatNormal?.Invoke(value);
        EventValueChangedFloatReversed?.Invoke(1f - value);
    }
    
    public void OnValueChangedBool(bool value)
    {
        EventValueChangedBoolNormal?.Invoke(value);
        EventValueChangedBoolReversed?.Invoke(!value);

        if (value)
        {
            EventPositive?.Invoke();
        }
    }

    private void Awake()
    {
        DIContainer.InjectTo(this);
        _gameEventService.Subscribe<GameEventTrigger>(OnTriggerReceived);
    }

    private void OnDestroy()
    {
        if (_gameEventService != null)
        {
            _gameEventService.Unsubscribe<GameEventTrigger>(OnTriggerReceived);
        }
    }

    private void OnTriggerReceived(GameEventTrigger triggerEvent)
    {
        if (triggerEvent.Trigger == trigger)
        {
            OnValueChangedFloat(triggerEvent.ValueFloat);
            OnValueChangedBool(triggerEvent.ValueBool);
        }
    }
}
