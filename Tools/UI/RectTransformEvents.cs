using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Larje.Core.Services.UI
{
    public class RectTransformEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Action<PointerEventData> PointerDown;
        public Action<PointerEventData> PointerMove;
        public Action<PointerEventData> PointerUp;
        public Action<PointerEventData> PointerEnter;
        public Action<PointerEventData> PointerExit;
        public Action<PointerEventData> PointerClick;


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
    }
}