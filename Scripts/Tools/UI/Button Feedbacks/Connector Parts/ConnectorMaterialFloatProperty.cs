using System.Collections.Generic;
using UnityEngine;

public class ConnectorMaterialFloatProperty : ButtonInteractionConnectorPart
{
    private Dictionary<string, List<float>> _properties = new Dictionary<string, List<float>>();
    
    public void Add(string property, float value)
    {
        if (!_properties.ContainsKey(property))
        {
            _properties.Add(property, new List<float>());
        }
        _properties[property].Add(value);
    }
    
    public override void Clear()
    {
        _properties.Clear();
    }

    public override void Apply(ButtonProperties properties)
    {
        foreach (string key in _properties.Keys)
        {
            float value = 1;
            foreach (float v in _properties[key])
            {
                value *= v;
            }

            if (properties.Material.HasFloat(key))
            {
                properties.Material.SetFloat(key, value);
            }
            
        }
    }
}
