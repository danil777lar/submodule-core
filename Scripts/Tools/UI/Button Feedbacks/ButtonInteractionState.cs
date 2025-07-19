using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ButtonInteractionState
{
    [SerializeField] public ButtonInteractionStateType stateType;
    [SerializeField, SerializeReference] public List<ButtonInteractionEffect> Effects = new List<ButtonInteractionEffect>();

    public void Evaluate(ButtonInteractionConnector connector, Dictionary<string, string> data)
    {
        foreach (ButtonInteractionEffect effect in Effects)
        {
            effect.Evaluate(connector, data);
        }
    }
}
