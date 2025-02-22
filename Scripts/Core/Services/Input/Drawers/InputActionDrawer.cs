using UnityEngine;
using UnityEngine.InputSystem;

public abstract class InputActionDrawer : ScriptableObject
{
    public abstract object Draw(InputAction action);
}
