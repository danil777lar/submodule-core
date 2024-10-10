using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Larje.Core.Services
{
    public class VirtualScreen : GraphicRaycaster
    {
        private Func<PointerEventData, VirtualScreenHit> _getCoords;

        public bool IsPointerOnScreen { get; private set; }
        public Vector2 PointerPosition { get; private set; }

        public void Initialize(Func<PointerEventData, VirtualScreenHit> getCoords)
        {
            _getCoords = getCoords;
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            VirtualScreenHit hit = _getCoords.Invoke(eventData);
            IsPointerOnScreen = hit.isHit;

            if (hit.isHit)
            {
                PointerPosition = hit.textureCoord;
                eventData.position = hit.textureCoord;
                base.Raycast(eventData, resultAppendList);
            }
        }

        public class VirtualScreenHit
        {
            public bool isHit;
            public Vector2 textureCoord;
        }
    }
}