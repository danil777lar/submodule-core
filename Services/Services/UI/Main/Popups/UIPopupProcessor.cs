using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    public class UIPopupProcessor
    {
        private Options _options;
        private Stack<UIPopup> _openedPopups = new Stack<UIPopup>();


        public UIPopupProcessor(Options options)
        {
            _options = options;
        }

        public UIPopup OpenPopup(PopupOpenProperties properties) 
        {
            UIPopup popupPrefab = _options.Popups.First(x => x.PopupType == properties.popupType);
            if (popupPrefab != null) 
            {
                if (_openedPopups.Count > 0)
                {
                    _openedPopups.Peek().PopupClosed -= OnLastPopupClosed;
                    HandleLastPopup(properties.combinationType);
                }
                _openedPopups.Push(GameObject.Instantiate(popupPrefab, _options.PopupHolder).Open(properties.popupArguments));
                _openedPopups.Peek().GetComponent<Canvas>().sortingOrder = _options.StartSortOrder + _openedPopups.Count;
                _openedPopups.Peek().PopupClosed += OnLastPopupClosed;
                return _openedPopups.Peek();
            }

            return null;
        }

        public bool TryClosePopupByBackButton() 
        {
            if (_openedPopups.Count > 0)
            {
                if (_openedPopups.Peek().CloseByBackDeviceKey)
                {
                    _openedPopups.Peek().Close();
                }
                return true;
            }
            else 
            {
                return false;
            }
        }

        private void HandleLastPopup(PopupCombinationType combinationType) 
        {
            switch (combinationType) 
            {
                case PopupCombinationType.Overlay:
                    break;
                case PopupCombinationType.Close:
                    _openedPopups.Peek().Close();
                    break;
                case PopupCombinationType.Hide:
                    _openedPopups.Peek().TryHide();
                    break;
            }
        }

        private void OnLastPopupClosed() 
        {
            _openedPopups.Pop();
            _openedPopups.Peek().TryShow();
        }


        [Serializable]
        public class Options
        {
            [SerializeField] private int _startSortOrder;
            [SerializeField] private Transform _popupHolder;
            [SerializeField] private UIPopup[] _popups;

            public int StartSortOrder => _startSortOrder;
            public Transform PopupHolder => _popupHolder;
            public UIPopup[] Popups => _popups;
        }
    }
}