using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractionConnector
{
    private ButtonProperties _properties;
    
    private List<ButtonInteractionConnectorPart> _parts = new List<ButtonInteractionConnectorPart>();
    
    public ButtonInteractionConnector(ButtonProperties properties)
    {
        _properties = properties;
    }

    public void Clear()
    {
        _parts.ForEach(part => part.Clear());
    }

    public void Apply()
    {
        _parts.ForEach(part => part.Apply(_properties));
    }

    public T GetPart<T>() where T : ButtonInteractionConnectorPart, new()
    {
        foreach (ButtonInteractionConnectorPart part in _parts)
        {
            if (part is T)
            {
                return part as T;
            }
        }

        T newPart = new T();
        _parts.Add(newPart);
        return newPart;
    }
}
