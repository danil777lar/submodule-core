using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Larje.Core.Services.UI
{
    public class UIPopupProcessor : UIProcessor
    {
        [Space]
        [SerializeField] private List<UIPopup> popups;
        
        private List<UIPopup> _openedPopups = new List<UIPopup>();

        public UIPopup OpenPopup(UIPopup.Args args) 
        {
            UIPopup popupPrefab = popups.First(x => x.PopupType == args.PopupType);
            if (popupPrefab != null) 
            {
                if (_openedPopups.Count > 0)
                {
                    HandleLastPopup(args.CombinationType);
                }

                UIPopup popupInstance = GameObject.Instantiate(popupPrefab, holder);
                popupInstance.Open(args);
                //popupInstance.GetComponent<Canvas>().sortingOrder = _options.StartSortOrder + _openedPopups.Count;
                popupInstance.EventClose += () => OnPopupClosed(popupInstance);
                _openedPopups.Add(popupInstance);
                
                return popupInstance;
            }

            return null;
        }

        public bool TryClosePopupByBackButton() 
        {
            if (_openedPopups.Count > 0)
            {
                if (_openedPopups.Last().CloseByBackDeviceKey)
                {
                    _openedPopups.Last().Close();
                }
                return true;
            }
            else 
            {
                return false;
            }
        }

        private void HandleLastPopup(UIPopupCombinationType combinationType) 
        {
            switch (combinationType) 
            {
                case UIPopupCombinationType.Overlay:
                    break;
                case UIPopupCombinationType.Close:
                    _openedPopups.Last().Close();
                    break;
                case UIPopupCombinationType.Hide:
                    _openedPopups.Last().Hide();
                    break;
            }
        }

        private void OnPopupClosed(UIPopup popup) 
        {
            if (_openedPopups.Count > 1 && _openedPopups.Last() == popup)
            {
                _openedPopups[^2].Show();
            }
            _openedPopups.Remove(popup);
        }
    }
}