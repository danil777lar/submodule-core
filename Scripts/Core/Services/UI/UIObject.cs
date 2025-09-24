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
        
        public event Action EventBeforeOpen;
        public event Action EventAfterOpen;
        public event Action EventBeforeClose;
        public event Action EventAfterClose;
        public event Action EventBeforeShow;
        public event Action EventAfterShow;
        public event Action EventBeforeHide;
        public event Action EventAfterHide;
        public event Action EventBeforeFocus;
        public event Action EventAfterFocus;
        public event Action EventBeforeUnfocus;
        public event Action EventAfterUnfocus;
        
        public virtual void Open(Args args)
        {
            if (!_opened)
            {
                _opened = true;
                
                _canvas = GetComponent<Canvas>();
                _eventDelays = new List<IUIObjectEventDelay>(GetComponentsInChildren<IUIObjectEventDelay>());

                OnBeforeOpen(args);
                EventBeforeOpen?.Invoke();

                float delay = GetDelay(x => x.OnOpen());
                DOVirtual.DelayedCall(delay, () =>
                {
                    OnAfterOpen(args);
                    EventAfterOpen?.Invoke();
                });
            }
        }
        
        public virtual void Close()
        {
            if (!_closed)
            {
                _closed = true;
                
                OnBeforeClose();
                EventBeforeClose?.Invoke();
            
                float delay = _hidden ? 0f : GetDelay(x => x.OnClose());
                DOVirtual.DelayedCall(delay, () =>
                {
                    OnAfterClose();
                    EventAfterClose?.Invoke();
                    
                    DestroyImmediate(gameObject);
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
                EventBeforeShow?.Invoke();

                float delay = GetDelay(x => x.OnShow());
                DOVirtual.DelayedCall(delay, () =>
                {
                    OnAfterShow();
                    EventAfterShow?.Invoke();
                });
            }
        }
        
        public virtual void Hide()
        {
            if (!_hidden && !_closed)
            {
                _hidden = true;
                
                OnBeforeHide();
                EventBeforeHide?.Invoke();
            
                float delay = GetDelay(x => x.OnHide());
                DOVirtual.DelayedCall(delay, () =>
                {
                    OnAfterHide();
                    EventAfterHide?.Invoke();
                    
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
                EventBeforeFocus?.Invoke();

                float delay = GetDelay(x => x.OnFocus());
                DOVirtual.DelayedCall(delay, () =>
                {
                    OnAfterFocus();
                    EventAfterFocus?.Invoke();
                });
            }
        }

        public virtual void Unfocus()
        {
            if (_focused && focusTarget && !_closed)
            {
                _focused = false;
                
                OnBeforeUnfocus();
                EventBeforeUnfocus?.Invoke();
            
                float delay = GetDelay(x => x.OnUnfocus());
                DOVirtual.DelayedCall(delay, () =>
                {
                    OnAfterUnfocus();
                    EventAfterUnfocus?.Invoke();
                });
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
        
        private float GetDelay(Func<IUIObjectEventDelay, float> delay)
        {
            return _eventDelays == null || _eventDelays.Count < 1 ? 0f : _eventDelays.Max(delay);
        }
        
        public class Args
        {
            
        }
    }
}
