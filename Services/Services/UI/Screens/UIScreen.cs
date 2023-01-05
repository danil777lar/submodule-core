using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIScreen : MonoBehaviour, IOpenCloseUI
    {
        [SerializeField] private UIScreenType _screenType;
        [SerializeField] private bool rewriteDeviceBackButton;
        public UIScreenType ScreenType => _screenType;
        public bool RewriteDeviceBackButton => rewriteDeviceBackButton;
        public event Action Opened;
        public event Action Closed;


        public UIScreen Open(Dictionary<string, object> args)
        {
            OnOpen(args);
            return this;
        }

        public void Close()
        {
            OnClose();
            float delay = 0f;
            IUIPartCloseDelay[] closeDelays = GetComponentsInChildren<IUIPartCloseDelay>();
            if (closeDelays.Length > 0)
            {
                delay = closeDelays.Max((delay) => delay.GetDelay());
            }
            Destroy(gameObject, delay);
        }

        protected virtual void OnOpen(Dictionary<string, object> args)
        {
            Opened?.Invoke();   
        }

        protected virtual void OnClose()
        {
            Closed?.Invoke();
        }

        protected virtual void OnDeviceBackButton()
        {
            
        }

        private void Update()
        {
            if (RewriteDeviceBackButton && Input.GetKeyDown(KeyCode.Escape))
            {
                OnDeviceBackButton();
            }
        }
    }
}