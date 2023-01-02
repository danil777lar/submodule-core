using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Larje.Core.Services.UI
{
    [RequireComponent(typeof(Canvas))]
    public sealed class UIToast : MonoBehaviour, IOpenCloseUI, IPointerDownHandler
    {
        [SerializeField] private UIToastType toastType;
        [SerializeField] private float closeDelay;
        [SerializeField] private int maxToastWidth;
        [Header("Links")]
        [SerializeField] private TextMeshProUGUI tmp;
        [SerializeField] private ContentSizeFitter sizeFitter;
        private Action _toastClosed;
        public UIToastType ToastType => toastType;
        
        public event Action Opened;
        public event Action Closed;

        public UIToast Open(string text, Action toastClosed)
        {
            _toastClosed = toastClosed;
            tmp.text = text;
            StartCoroutine(OpenCoroutine());
            StartCoroutine(CloseDelayCoroutine());
            Opened?.Invoke();
            return this;
        }

        public void Close()
        {
            _toastClosed?.Invoke();
            Closed?.Invoke();
            float delay = 0f;
            IUIPartCloseDelay[] closeDelays = GetComponentsInChildren<IUIPartCloseDelay>();
            if (closeDelays.Length > 0) 
            {
                delay = closeDelays.Max((delay) => delay.GetDelay());
            }
            Destroy(gameObject, delay);
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
    }
}