using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class UIObject : MonoBehaviour
    {
        [SerializeField] private bool useDeviceBackButton = true;
        [SerializeField] private bool blockNextDeviceBackButtonInvokes = true;
        [SerializeField] private bool focusTarget = true;
        
        private bool _opened;
        private bool _closed;
        private bool _hidden;
        private bool _focused;
        
        private Canvas _canvas;
        private List<IUIObjectEventDelay> _eventDelays = new List<IUIObjectEventDelay>();
        
        public bool Opened => _opened;
        public bool Closed => _closed;
        public bool Hidden => _hidden;
        public bool Focused => _focused;
        public bool FocusTarget => focusTarget;

        public Canvas Canvas => _canvas;
        
        public event Action EventOpen;
        public event Action EventClose;
        public event Action EventShow;
        public event Action EventHide;
        public event Action EventFocus;
        public event Action EventUnfocus;
        
        public virtual void Open(Args args)
        {
            if (!_opened)
            {
                _opened = true;
                
                _canvas = GetComponent<Canvas>();
                _eventDelays = new List<IUIObjectEventDelay>(GetComponentsInChildren<IUIObjectEventDelay>());

                OnBeforeOpen(args);
                EventOpen?.Invoke();

                float delay = _eventDelays.Max(x => x.OnOpen());
                DOVirtual.DelayedCall(delay, () => OnAfterOpen(args));
                Debug.Log("Open Delay: " + delay);
            }
        }
        
        public virtual void Close()
        {
            if (!_closed)
            {
                _closed = true;
                
                OnBeforeClose();
            
                float delay = _hidden ? 0f : _eventDelays.Max(x => x.OnClose());
                DOVirtual.DelayedCall(delay, () =>
                {
                    OnAfterClose();
                    DestroyImmediate(gameObject);
                    EventClose?.Invoke();
                });
            }
        }
        
        public virtual void Show()
        {
            if (_hidden && !_closed)
            {
                _hidden = false;
                
                gameObject.SetActive(true);
                OnBeforeShow();
                EventShow?.Invoke();

                float delay = _eventDelays.Max(x => x.OnShow());
                DOVirtual.DelayedCall(delay, OnAfterShow);
            }
        }
        
        public virtual void Hide()
        {
            if (!_hidden && !_closed)
            {
                _hidden = true;
                
                OnBeforeHide();
                EventHide?.Invoke();
            
                float delay = _eventDelays.Max(x => x.OnHide());
                DOVirtual.DelayedCall(delay, () =>
                {
                    OnAfterHide();
                    gameObject.SetActive(false);
                });
            }
        }
        
        public virtual void Focus()
        {
            if (!_focused && focusTarget && !_closed)
            {
                _focused = true;
                
                OnBeforeFocus();
                EventFocus?.Invoke();

                float delay = _eventDelays.Max(x => x.OnFocus());
                DOVirtual.DelayedCall(delay, OnAfterFocus);
            }
        }

        public virtual void Unfocus()
        {
            if (_focused && focusTarget && !_closed)
            {
                _focused = false;
                
                OnBeforeUnfocus();
                EventUnfocus?.Invoke();
            
                float delay = _eventDelays.Max(x => x.OnUnfocus());
                DOVirtual.DelayedCall(delay, OnAfterUnfocus);
            }
        }

        public void SetSortingOrder(int sortingOrder)
        {
            if (_canvas != null)
            {
                _canvas.sortingOrder = sortingOrder;
            }
        }
        
        public bool Back(bool onlyOverride = false)
        {
            if (useDeviceBackButton)
            {
                return OnBack(onlyOverride) && blockNextDeviceBackButtonInvokes;
            }
            return false;
        }
        
        protected virtual void OnBeforeOpen(Args args) {}
        
        protected virtual void OnAfterOpen(Args args) {}
        
        protected virtual void OnBeforeClose() {}
        
        protected virtual void OnAfterClose() {}

        protected virtual void OnBeforeShow() {}

        protected virtual void OnAfterShow() {}
        
        protected virtual void OnBeforeHide() {}
        
        protected virtual void OnAfterHide() {}

        protected virtual void OnBeforeFocus() {}

        protected virtual void OnAfterFocus() {}

        protected virtual void OnBeforeUnfocus() {}
        
        protected virtual void OnAfterUnfocus() {}

        protected virtual bool OnBack(bool onlyOverride)
        {
            if (_opened && !_closed && !onlyOverride)
            {
                Close();
                return true;
            }
            
            return false;
        }
        
        public class Args
        {
            
        }
    }
}
