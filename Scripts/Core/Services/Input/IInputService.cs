using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputService
{
    public InputSystem_Actions.PlayerActions PlayerActions { get; }
}
