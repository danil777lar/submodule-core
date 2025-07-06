using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ButtonInteractionState
{
    [SerializeField] public float durationIn = 0.25f;
    [SerializeField] public float durationOut = 0.25f;
    
    [SerializeField] public ButtonInteractionStateType stateType;
    [SerializeField, SerializeReference] public List<ButtonInteractionEffect> Effects = new List<ButtonInteractionEffect>();

    public void Evaluate(ButtonInteractionConnector connector, float t)
    {
        foreach (ButtonInteractionEffect effect in Effects)
        {
            effect.Evaluate(connector, t);
        }
    }
}
