using System;
using System.Collections;
using System.Linq;
using ProjectConstants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Larje.Core.Services.UI
{
    [RequireComponent(typeof(Canvas))]
    public sealed class UIToast : UIObject, IPointerDownHandler
    {
        [SerializeField] private UIToastType toastType;
        [SerializeField] private float closeDelay;
        [SerializeField] private int maxToastWidth;
        [Header("Links")]
        [SerializeField] private TextMeshProUGUI tmp;
        [SerializeField] private ContentSizeFitter sizeFitter;
        
        public UIToastType ToastType => toastType;

        protected override void OnBeforeOpen(UIObject.Args args)
        {
            if (args is Args toastArgs)
            {
                tmp.text = toastArgs.Text;
                StartCoroutine(OpenCoroutine());
                
                if (toastArgs.IsOpen == null)
                {
                    StartCoroutine(CloseDelayCoroutine());
                }
                else
                {
                    StartCoroutine(CloseWaitCoroutine(toastArgs.IsOpen));
                }
            }   
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            Close();
        }

        private IEnumerator OpenCoroutine()
        {
            yield return null;
            yield return null;
            
            RectTransform rect = (RectTransform)tmp.transform; 
            if (rect.sizeDelta.x > maxToastWidth)
            {
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                rect.sizeDelta = new Vector2(maxToastWidth, rect.sizeDelta.y);
            }
        }

        private IEnumerator CloseDelayCoroutine()
        {
            yield return new WaitForSeconds(closeDelay);
            Close();
        }
        
        private IEnumerator CloseWaitCoroutine(Func<bool> isOpen)
        {
            yield return null;
            while (isOpen.Invoke())
            {
                yield return null;
            }
            Close();
        }
        
        public class Args : UIObject.Args
        {
            public readonly UIToastType ToastType;
            public readonly string Text;
            public readonly Func<bool> IsOpen;

            public Args(UIToastType toastType, string text, Func<bool> isOpen = null)
            {
                ToastType = toastType;
                Text = text;
                IsOpen = isOpen;
            }
        }
    }
}