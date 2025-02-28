using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Larje.Core.Services.DebugConsole
{
    public class DebugConsoleOpenCloseButton : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerDownHandler
    {
        private bool _canBeClicked;
        private DebugConsoleService _debugConsoleService;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_canBeClicked)
            {
                _debugConsoleService.OpenConsole();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
            _canBeClicked = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _canBeClicked = true;
        }

        private void Start()
        {
            _debugConsoleService = GetComponentInParent<DebugConsoleService>();
        }
    }
}