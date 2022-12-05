using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    public class ScreenOpenProperties
    {
        public UIScreenType screenType;
        public Dictionary<string, object> screenArguments = new Dictionary<string, object>();

        public ScreenOpenProperties(UIScreenType screenType)
        {
            this.screenType = screenType;
        }
    }
}
