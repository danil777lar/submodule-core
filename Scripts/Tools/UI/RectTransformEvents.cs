using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Larje.Core.Services.UI
{
    public class RectTransformEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler
    {
        public event Action<PointerEventData> EventPointerDown;
        public event Action<PointerEventData> EventPointerMove;
        public event Action<PointerEventData> EventPointerUp;
        public event Action<PointerEventData> EventPointerEnter;
        public event Action<PointerEventData> EventPointerExit;
        public event Action<PointerEventData> EventPointerClick;
        public event Action<PointerEventData> EventPointerDrag;


        public void OnPointerDown(PointerEventData eventData)
        {
            EventPointerDown?.Invoke(eventData);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            EventPointerMove?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            EventPointerUp?.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            EventPointerEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            EventPointerExit?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            EventPointerClick?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            EventPointerDrag?.Invoke(eventData);
        }
    }
}