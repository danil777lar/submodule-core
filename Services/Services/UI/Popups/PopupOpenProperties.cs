using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    public class PopupOpenProperties
    {
        public readonly UIPopupType popupType;
        public PopupCombinationType combinationType = PopupCombinationType.Close;
        public Dictionary<string, object> popupArguments = new Dictionary<string, object>();

        public PopupOpenProperties(UIPopupType popupType)
        {
            this.popupType = popupType;
        }
    }

    public enum PopupCombinationType
    {
        Close,
        Overlay,
        Hide
    }
}