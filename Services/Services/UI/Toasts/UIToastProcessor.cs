using System;
using System.Linq;
using ProjectConstants;
using UnityEngine;
using UnityEngine.Serialization;

namespace Larje.Core.Services.UI
{
    public class UIToastProcessor : UIProcessor
    {
        [Space]
        [SerializeField] private UIToast[] toasts;
        
        private UIToast _openedToast;

        public void OpenToast(UIToast.Args args)
        {
            UIToast toast = toasts.First(x => x.ToastType == args.ToastType);
            if (toast != null)
            {
                if (_openedToast != null)
                {
                    _openedToast.Close();
                }
                
                UIToast toastInstance = GameObject.Instantiate(toast, holder);
                toastInstance.Open(args);
                toastInstance.EventClose += () => OnToastClosed(toastInstance);
                
                _openedToast = toastInstance;
                
                AddOpenedUIObject(_openedToast);
            }
        }
        
        public override bool ComputeDeviceBackButton()
        {
            if (_openedToast != null)
            {
                return _openedToast.ComputeDeviceBackButton();
            }
            
            return false;
        }
        
        private void OnToastClosed(UIToast toast)
        {
            _openedToast = null;
            RemoveOpenedUIObject(toast);
        }
    }
}