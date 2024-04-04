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
                popupInstance.EventClose += () => OnPopupClosed(popupInstance);
                _openedPopups.Add(popupInstance);
                AddOpenedUIObject(popupInstance);
                
                return popupInstance;
            }

            return null;
        }

        public override bool Back() 
        {
            for (int i = _openedPopups.Count - 1; i >= 0; i--)
            {
                bool result = _openedPopups[i].Back();
                if (result)
                {
                    return true;
                }   
            }
            
            return false;
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
            RemoveOpenedUIObject(popup);
        }
    }
}