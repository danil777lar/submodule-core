using System;
using System.Linq;
using UnityEngine;

public abstract class UIAnimationBase : MonoBehaviour, IUIObjectEventDelay
{
    [SerializeField] private EventType[] eventTypes = new []
    {
        EventType.Root_Open,
        EventType.Root_Close,
    };
    
    public float OnOpen()
    {
        return ProcessEvent(EventType.Root_Open, true);
    }

    public float OnClose()
    {
        return ProcessEvent(EventType.Root_Close, false);
    }

    public float OnShow()
    {
        return ProcessEvent(EventType.Root_Show, true);
    }

    public float OnHide()
    {
        return ProcessEvent(EventType.Root_Hide, false);
    }

    public float OnFocus()
    {
        return ProcessEvent(EventType.Root_Focus, true);
    }

    public float OnUnfocus()
    {
        return ProcessEvent(EventType.Root_Unfocus, false);
    }

    public float PointerDown()
    {
        return ProcessEvent(EventType.Pointer_Down, true);
    }

    public float PointerUp()
    {
        return ProcessEvent(EventType.Pointer_Up, false);
    }

    public float PointerEnter()
    {
        return ProcessEvent(EventType.Pointer_Enter, true);
    }

    public float PointerExit()
    {
        return ProcessEvent(EventType.Pointer_Exit, false);
    }

    public float PointerClick()
    {
        return ProcessEvent(EventType.Pointer_Click, true);
    }

    public float SelectableSelect()
    {
        return ProcessEvent(EventType.Selectable_Select, true);
    }

    public float SelectableDeselect()
    {
        return ProcessEvent(EventType.Selectable_Deselect, false);
    }

    public float SelectableInteractableOn()
    {
        return ProcessEvent(EventType.Selectable_InteractableOn, true);
    }

    public float SelectableInteractableOff()
    {
        return ProcessEvent(EventType.Selectable_InteractableOff, false);
    }

    public float DraggableBeginDrag()
    {
        return ProcessEvent(EventType.Draggable_BeginDrag, true);
    }

    public float DraggableEndDrag()
    {
        return ProcessEvent(EventType.Draggable_EndDrag, false);
    }

    public float GameObjectEnable()
    {
        return ProcessEvent(EventType.GameObject_Enable, true);
    }

    public float GameObjectDisable()
    {
        return ProcessEvent(EventType.GameObject_Disable, false);
    }

    protected float ProcessEvent(EventType type, bool forward)
    {
        if (eventTypes.ToList().Contains(type))
        {
            return OnEvent(forward);
        }

        return 0f;
    }

    protected abstract float OnEvent(bool forward);
    
    protected enum EventType 
    {
        Root_Open = 101, 
        Root_Close = 102, 
        Root_Show = 103,
        Root_Hide = 104,
        Root_Focus = 105,
        Root_Unfocus = 106,

        Pointer_Down = 201,
        Pointer_Up = 202,
        Pointer_Enter = 203,
        Pointer_Exit = 204,
        Pointer_Click = 205,

        Selectable_Select = 301,
        Selectable_Deselect = 302,
        Selectable_InteractableOn = 303,
        Selectable_InteractableOff = 304,

        Draggable_BeginDrag = 401,
        Draggable_EndDrag = 402,

        GameObject_Enable = 501,
        GameObject_Disable = 502,
    }
}
