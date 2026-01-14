using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    public interface IAnalyticsService
    {
        public void SendEvent(string eventName);
    }
}