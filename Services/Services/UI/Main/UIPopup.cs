using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    public class UIPopup : MonoBehaviour
    {
        [SerializeField] private UIPopupType _popupType;

        public UIPopupType PopupType => _popupType;
    }
}