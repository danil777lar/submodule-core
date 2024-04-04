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

        public override void Open(UIObject.Args args)
        {
            if (!Opened && args is Args toastArgs)
            {
                tmp.text = toastArgs.Text;
                StartCoroutine(OpenCoroutine());
                StartCoroutine(CloseDelayCoroutine());
            }
            
            base.Open(args);
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
        
        public class Args : UIObject.Args
        {
            public readonly UIToastType ToastType;
            public readonly string Text;

            public Args(UIToastType toastType, string text)
            {
                ToastType = toastType;
                Text = text;
            }
        }
    }
}