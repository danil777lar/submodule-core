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
                _openedToast = GameObject.Instantiate(toast, holder);
                _openedToast.Open(args);
                _openedToast.EventClose += () => _openedToast = null;
            }
        }
    }
}