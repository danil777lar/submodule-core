using System;
using Larje.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public abstract class TriggerSender : MonoBehaviour
{
    [Header("Trigger Settings")] 
    [SerializeField] private string key;
    [SerializeField] private TriggerConstant trigger;
    [Space]
    [SerializeField] private bool sendEventOnStart = true;
    [SerializeField, Range(0f, 1f)] private float threshold = 1f;

    [InjectService] private GameEventService _gameEventService;
    
    private float _value;

    public float Value
    {
        get => gameObject.activeSelf && gameObject.activeInHierarchy ? _value : 0f;
        protected set
        {
            _value = value;
            if (trigger != null)
            {
                _gameEventService.SendEvent(new GameEventTrigger(trigger, _value, gameObject.name));
            }

            EventValueChanged?.Invoke(_value);
        }
    }

    public event Action<float> EventValueChanged;

    protected virtual void Start()
    {
        DIContainer.InjectTo(this, typeof(TriggerSender));

        if (sendEventOnStart)
        {
            Value = _value;
        }
    }
}
