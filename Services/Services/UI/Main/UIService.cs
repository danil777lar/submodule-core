using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    [BindService(typeof(UIService))]
    public class UIService : Service
    {
        [SerializeField] private bool _useBackButton = true;
        [SerializeField] private bool _saveFirstScreen = false;
        [Space]
        [SerializeField] private Transform _screenHolder;
        [SerializeField] private Transform _popupHolder;
        [Space]
        [SerializeField] private UIScreenType _defaultScreenType;
        [Space]
        [SerializeField] private List<UIScreen> _screens;
        [SerializeField] private List<UIPopup> _popups;

        private UIScreen _openedScreen;
        private ScreenOpenProperties _openedScreenProperties;
        private Stack<ScreenOpenProperties> _history;

        public Action<UIScreenType, UIScreenType> ScreenChanged;


        private void Awake()
        {
            _history = new Stack<ScreenOpenProperties>();
            ShowScreen(new ScreenOpenProperties(_defaultScreenType), true, _saveFirstScreen);
        }

        private void Update()
        {
            if (_useBackButton) 
            {
                if (Input.GetKeyDown(KeyCode.Escape)) 
                {
                    ShowPreviousScreen();
                }
            }
        }

        public void ShowScreen(ScreenOpenProperties args, bool pushToHistory = true, bool saveProperties = true) 
        {
            UIScreen screenToOpen = _screens.Find((screen) => screen.ScreenType == args.screenType);
            if (screenToOpen != null)
            {
                UIScreen screenToClose = _openedScreen;
                if (_openedScreenProperties != null && pushToHistory) 
                {
                    _history.Push(_openedScreenProperties); 
                }
                if (saveProperties)
                {
                    _openedScreenProperties = args;
                }

                _openedScreen = Instantiate(screenToOpen, _screenHolder).Open(args.screenArguments);

                if (screenToClose) 
                {
                    ScreenChanged?.Invoke(_openedScreen.ScreenType, args.screenType);
                    screenToClose.Close();
                }
            }
        }

        public void ShowPreviousScreen() 
        {
            if (_history.Count > 0)
            {
                ShowScreen(_history.Pop(), false);
            }
        }

        public void ShowPopup(UIPopupType popupType) 
        {

        }

        public override void Init(){}


        public class ScreenOpenProperties
        {
            public readonly UIScreenType screenType;
            public readonly object[] screenArguments;

            public ScreenOpenProperties(UIScreenType screenType, object[] screenArguments = null) 
            {
                this.screenType = screenType;
                this.screenArguments = screenArguments;
            }
        }
    }
}