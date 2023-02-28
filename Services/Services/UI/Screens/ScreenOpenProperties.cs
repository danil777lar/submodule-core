using System.Collections;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    public class ScreenOpenProperties
    {
        public UIScreenType screenType;

        public ScreenOpenProperties(UIScreenType screenType)
        {
            this.screenType = screenType;
        }
    }
}
