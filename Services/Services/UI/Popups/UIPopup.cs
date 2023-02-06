using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIPopup : MonoBehaviour, IOpenCloseUI, IShowHideUI
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

        public event Action Opened;
        public event Action Closed;
        public event Action Shown;
        public event Action Hidden;

        public UIPopup Open(Dictionary<string, object> args)
        {
            OnOpened(args);

            if (_closeByBackgroundClick)
            {
                _background.PointerClick += (x) => Close();
            }

            return this;
        }

        public void Close()
        {
            OnClosed();
            float delay = 0f;
            IUIPartCloseDelay[] closeDelays = GetComponentsInChildren<IUIPartCloseDelay>();
            if (closeDelays.Length > 0) 
            {
                delay = closeDelays.Max((delay) => delay.GetDelay());
            }
            Destroy(gameObject, delay);
        }

        public void TryHide() 
        {
            if (!_isHidden) 
            {
                OnHidden();
                _isHidden = true;
                gameObject.SetActive(false);
            }
        }

        public void TryShow() 
        {
            if (_isHidden) 
            {
                OnShowed();
                _isHidden = false;
                gameObject.SetActive(true);
            }
        }

        protected virtual void OnOpened(Dictionary<string, object> args)
        {
            Opened?.Invoke();
        }

        protected virtual void OnClosed()
        {
            Closed?.Invoke();
        }
        
        protected virtual void OnHidden()
        {
            Hidden?.Invoke();
        }

        protected virtual void OnShowed()
        {
            Shown?.Invoke();
        }
    }
}