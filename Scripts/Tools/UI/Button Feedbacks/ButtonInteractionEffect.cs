using System;
using UnityEngine;

[Serializable]
public abstract class ButtonInteractionEffect
{
    public abstract void Evaluate(ButtonInteractionConnector connector, float t);
}
