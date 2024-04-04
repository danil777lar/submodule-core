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
        [SerializeField] private bool blockDeviceBackButtonInvoke = true;
        
        private bool _opened;
        private bool _closed;
        private bool _hidden;
        private bool _focused;
        private List<IUIObjectEventDelay> _eventDelays = new List<IUIObjectEventDelay>();
        
        public bool Opened => _opened;
        public bool Closed => _closed;
        public bool Hidden => _hidden;
        public bool Focused => _focused;
        
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
                
                _eventDelays = new List<IUIObjectEventDelay>(GetComponentsInChildren<IUIObjectEventDelay>());

                OnBeforeOpen(args);
                EventOpen?.Invoke();

                this.DOKill();
                DOTween.Sequence()
                    .SetTarget(this)
                    .AppendInterval(_eventDelays.Max(x => x.OnOpen()))
                    .AppendCallback(() => OnAfterOpen(args));
            }
        }
        
        public virtual void Close()
        {
            if (!_closed)
            {
                _closed = true;
                
                OnBeforeClose();
                EventClose?.Invoke();
            
                this.DOKill();
                DOTween.Sequence()
                    .SetTarget(this)
                    .AppendInterval(_eventDelays.Max(x => x.OnClose()))
                    .AppendCallback(() =>
                    {
                        OnAfterClose();
                        Destroy(gameObject);
                    });   
            }
        }
        
        public virtual void Show()
        {
            if (_hidden)
            {
                _hidden = false;
                
                gameObject.SetActive(true);
                OnBeforeShow();
                EventShow?.Invoke();

                this.DOKill();
                DOTween.Sequence()
                    .SetTarget(this)
                    .AppendInterval(_eventDelays.Max(x => x.OnShow()))
                    .AppendCallback(OnAfterShow);   
            }
        }
        
        public virtual void Hide()
        {
            if (!_hidden)
            {
                _hidden = true;
                
                OnBeforeHide();
                EventHide?.Invoke();
            
                this.DOKill();
                DOTween.Sequence()
                    .SetTarget(this)
                    .AppendInterval(_eventDelays.Max(x => x.OnHide()))
                    .AppendCallback(() =>
                    {
                        OnAfterHide();
                        gameObject.SetActive(false);
                    });   
            }
        }
        
        public virtual void Focus()
        {
            if (!_focused)
            {
                _focused = true;
                
                OnBeforeFocus();
                EventFocus?.Invoke();

                this.DOKill();
                DOTween.Sequence()
                    .SetTarget(this)
                    .AppendInterval(_eventDelays.Max(x => x.OnFocus()))
                    .AppendCallback(OnAfterFocus);   
            }
        }

        public virtual void Unfocus()
        {
            if (_focused)
            {
                _focused = false;
                
                OnBeforeUnfocus();
                EventUnfocus?.Invoke();
            
                this.DOKill();
                DOTween.Sequence()
                    .SetTarget(this)
                    .AppendInterval(_eventDelays.Max(x => x.OnUnfocus()))
                    .AppendCallback(OnAfterUnfocus);   
            }
        }
        
        public bool ComputeDeviceBackButton()
        {
            if (useDeviceBackButton)
            {
                return OnDeviceBackButton() && blockDeviceBackButtonInvoke;
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

        protected virtual bool OnDeviceBackButton()
        {
            if (_opened && !_closed)
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
