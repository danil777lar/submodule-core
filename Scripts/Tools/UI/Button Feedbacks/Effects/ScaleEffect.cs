using UnityEngine;

public class ScaleEffect : ButtonInteractionEffect
{
    public override void Evaluate(ButtonInteractionConnector connector, float t)
    {
        Vector3 value = Vector3.one;
        connector.GetPart<ConnectorScale>().Add(value);
    }
}
