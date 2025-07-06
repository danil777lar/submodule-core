using UnityEngine;

public class ButtonProperties
{
    public Transform Transform { get; private set; }
    public Material Material { get; private set; }
    
    public ButtonProperties(Transform transform, Material material)
    {
        Transform = transform;
        Material = material;
    }
}
