using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    public class UIScreenProcessor
    {
        private Options _options;
        private UIScreen _openedScreen;
        private ScreenOpenProperties _openedScreenProperties;
        private Stack<ScreenOpenProperties> _history = new Stack<ScreenOpenProperties>();

        public Action<UIScreenType, UIScreenType> ScreenChanged;


        public UIScreenProcessor(Options options)
        {
            _options = options;
        }


        public void OpenScreen(ScreenOpenProperties args, bool pushToHistory = true, bool saveProperties = true)
        {
            UIScreen screenToOpen = _options.Screens.First((screen) => screen.ScreenType == args.screenType);
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

                _openedScreen = GameObject.Instantiate(screenToOpen, _options.ScreenHolder).Open(args.screenArguments);
                _openedScreen.GetComponent<Canvas>().sortingOrder = _options.StartSortOrder;

                if (screenToClose)
                {
                    ScreenChanged?.Invoke(screenToClose.ScreenType, args.screenType);
                    screenToClose.Close();
                }
            }
        }

        public bool ComputeDeviceBackButton()
        {
            if (!_openedScreen.RewriteDeviceBackButton)
            {
                return TryOpenPreviousScreen();
            }
            return false;
        }
        
        public bool TryOpenPreviousScreen()
        {
            if (_history.Count > 0)
            {
                OpenScreen(_history.Pop(), false);
                return true;
            }
            return false;
        }


        [Serializable]
        public class Options 
        {
            [SerializeField] private int _startSortOrder;
            [SerializeField] private Transform _screenHolder;
            [SerializeField] private UIScreen[] _screens;

            public int StartSortOrder => _startSortOrder;
            public Transform ScreenHolder => _screenHolder;
            public UIScreen[] Screens => _screens;
        }
    }
}