using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    [BindService(typeof(UIService))]
    public class UIService : Service
    {
        [SerializeField] private Transform _screenHolder;
        [SerializeField] private Transform _popupHolder;
        [Space]
        [SerializeField] private UIScreenType _defaultScreenType;
        [Space]
        [SerializeField] private List<UIScreen> _screens;
        [SerializeField] private List<UIPopup> _popups;

        private UIScreen _openedScreen;


        private void Awake()
        {
            ShowScreen(_defaultScreenType, false);
        }

        public void ShowScreen(UIScreenType screenType, bool withAnim = true) 
        {
            UIScreen screenToOpen = _screens.Find((screen) => screen.ScreenType == screenType);
            if (screenToOpen != null)
            {
                _openedScreen?.Close();
                _openedScreen = Instantiate(screenToOpen, _screenHolder).Open();
            }
        }

        public void ShowPopup(UIPopupType popupType) 
        {

        }

        public override void Init(){}
    }
}