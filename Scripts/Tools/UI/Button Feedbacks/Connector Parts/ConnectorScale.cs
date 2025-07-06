using System.Collections.Generic;
using UnityEngine;

public class ConnectorScale : ButtonInteractionConnectorPart
{
    private List<Vector3> _scales = new List<Vector3>();
    
    public void Add(Vector3 scale)
    {
        _scales.Add(scale);
    }
    
    public override void Clear()
    {
        _scales.Clear();
    }

    public override void Apply(ButtonProperties properties)
    {
        Vector3 finalScale = Vector3.one;
        foreach (Vector3 scale in _scales)
        {
            finalScale = Vector3.Scale(finalScale, scale);
        }
        
        properties.Transform.localScale = finalScale;
    }
}
