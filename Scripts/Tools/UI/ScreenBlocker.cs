using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services.UI;
using UnityEngine;

public class ScreenBlocker : MonoBehaviour
{
    
    
    private void Awake()
    {
        UIScreen screen = GetComponentInParent<UIScreen>();
        
        screen.EventBeforeOpen += Activate;
        screen.EventBeforeShow += Activate;
        
        screen.EventAfterOpen += Deactivate;
        screen.EventAfterShow += Deactivate;
    }

    private void Activate()
    {
        gameObject.SetActive(true);
    }
    
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
