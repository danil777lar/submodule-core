using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Input Action Drawer Text", menuName = "Larje/Core/Services/Input/Drawers/Text", order = 1)]
public class InputActionDrawerText : InputActionDrawer
{
    [SerializeField] private string leftModifier;
    [SerializeField] private string rightModifier;
    
    public override object Draw(InputAction action)
    {
        string title = "";

        if (action.type == InputActionType.Button)
        {
            title = action.bindings.First().path;
            title = title.Split('/').Last();
        }

        return $"{leftModifier}{title}{rightModifier}";
    }
}
