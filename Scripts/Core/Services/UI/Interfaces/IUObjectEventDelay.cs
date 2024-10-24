using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIObjectEventDelay
{
    float OnOpen();
    float OnClose();
    float OnShow();
    float OnHide();
    float OnFocus();
    float OnUnfocus();
}
