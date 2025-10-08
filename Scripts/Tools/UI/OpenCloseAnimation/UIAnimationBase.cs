using System;
using UnityEngine;

public abstract class UIAnimationBase : MonoBehaviour, IUIObjectEventDelay
{
    [SerializeField] private EventType eventType = EventType.Open | EventType.Close;
    
    public float OnOpen()
    {
        return ProcessEvent(EventType.Open, true);
    }

    public float OnClose()
    {
        return ProcessEvent(EventType.Close, false);
    }

    public float OnShow()
    {
        return ProcessEvent(EventType.Show, true);
    }

    public float OnHide()
    {
        return ProcessEvent(EventType.Hide, false);
    }

    public float OnFocus()
    {
        return ProcessEvent(EventType.Focus, true);
    }

    public float OnUnfocus()
    {
        return ProcessEvent(EventType.Unfocus, false);
    }

    protected float ProcessEvent(EventType type, bool forward)
    {
        if (eventType.HasFlag(type))
        {
            return OnEvent(forward);
        }

        return 0f;
    }

    protected abstract float OnEvent(bool forward);
    
    [Flags]
    protected enum EventType 
    {
        Open = 1, 
        Close = 2, 
        Show = 4,
        Hide = 8,
        Focus = 16,
        Unfocus = 32
    }
}
