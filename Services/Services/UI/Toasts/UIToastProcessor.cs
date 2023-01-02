using System;
using System.Linq;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    public class UIToastProcessor
    {
        private Options _options;
        private UIToast _openedToast;


        public UIToastProcessor(Options options)
        {
            _options = options;
        }

        public void ShowToast(UIToastType toastType, string text)
        {
            UIToast toast = _options.Toasts.First(x => x.ToastType == toastType);
            if (toast != null)
            {
                if (_openedToast != null)
                {
                    _openedToast.Close();
                }
                _openedToast = GameObject.Instantiate(toast, _options.ToastHolder).Open(text, () => _openedToast = null);
            }
        }


        [Serializable]
        public class Options
        {
            [SerializeField] private int startSortOrder;
            [SerializeField] private Transform toastsHolder;
            [SerializeField] private UIToast[] toasts;

            public int StartSortOrder => startSortOrder;
            public Transform ToastHolder => toastsHolder;
            public UIToast[] Toasts => toasts;

        }
    }
}