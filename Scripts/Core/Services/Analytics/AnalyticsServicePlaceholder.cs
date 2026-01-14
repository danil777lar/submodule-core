using Larje.Core;
using Larje.Core.Services;
using UnityEngine;

[BindService(typeof(IAnalyticsService))]
public class AnalyticsServicePlaceholder : Service, IAnalyticsService
{
    public override void Init()
    {
        
    }

    public void SendEvent(string eventName)
    {
        Debug.Log("AnalyticsServicePlaceholder | SendEvent: " + eventName);
    }
}
