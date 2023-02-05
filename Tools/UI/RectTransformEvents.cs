using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Larje.Core.Services.UI
{
    public class RectTransformEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler
    {
        public event Action<PointerEventData> PointerDown;
        public event Action<PointerEventData> PointerMove;
        public event Action<PointerEventData> PointerUp;
        public event Action<PointerEventData> PointerEnter;
        public event Action<PointerEventData> PointerExit;
        public event Action<PointerEventData> PointerClick;
        public event Action<PointerEventData> PointerDrag;


        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke(eventData);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            PointerMove?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUp?.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerExit?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PointerClick?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            PointerDrag?.Invoke(eventData);
        }
    }
}