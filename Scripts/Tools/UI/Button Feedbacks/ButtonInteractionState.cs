using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ButtonInteractionState
{
    public float durationIn = 0.25f;
    public float durationOut = 0.25f;
    
    public ButtonInteractionStateType stateType;
    public List<ButtonInteractionEffect> Effects = new List<ButtonInteractionEffect>();

    public void Evaluate(ButtonInteractionConnector connector, float t)
    {
        foreach (ButtonInteractionEffect effect in Effects)
        {
            effect.Evaluate(connector, t);
        }
    }
}
