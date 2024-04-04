using System;
using System.Linq;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    public class UIToastProcessor : UIProcessor
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
                _openedToast = GameObject.Instantiate(toast, _options.Holder).Open(text, () => _openedToast = null);
            }
        }


        [Serializable]
        public class Options : UIProcessor.Options
        {
            [field: SerializeField] public UIToast[] Toasts { get; private set; }
        }
    }
}