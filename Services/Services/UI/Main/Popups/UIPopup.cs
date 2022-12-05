using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIPopup : MonoBehaviour
    {
        [SerializeField] private UIPopupType _popupType;
        [Space]
        [SerializeField] private bool _closeByBackDeviceKey;
        [SerializeField] private bool _closeByBackgroundClick;
        [Space]
        [SerializeField] private RectTransformEvents _background;
        private bool _isHidden;

        public bool CloseByBackDeviceKey => _closeByBackDeviceKey;
        public UIPopupType PopupType => _popupType;

        public Action<Dictionary<string, object>> PopupOpened;
        public Action PopupClosed;
        public Action PopupHidden;
        public Action PopupShowed;


        public UIPopup Open(Dictionary<string, object> arguments)
        {
            PopupOpened?.Invoke(arguments);

            if (_closeByBackgroundClick)
            {
                _background.PointerClick += (x) => Close();
            }

            return this;
        }

        public void Close()
        {
            PopupClosed?.Invoke();
            float delay = GetComponentsInChildren<IUIPartCloseDelay>().Max((delay) => delay.GetDelay());
            Destroy(gameObject, delay);
        }

        public void TryHide() 
        {
            if (!_isHidden) 
            {
                PopupHidden?.Invoke();
                _isHidden = true;
                gameObject.SetActive(false);
            }
        }

        public void TryShow() 
        {
            if (_isHidden) 
            {
                PopupShowed?.Invoke();
                _isHidden = false;
                gameObject.SetActive(true);
            }
        }
    }
}