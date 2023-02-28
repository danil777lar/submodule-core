using System.Collections;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    public class PopupOpenProperties
    {
        public readonly UIPopupType popupType;
        public PopupCombinationType combinationType = PopupCombinationType.Close;

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