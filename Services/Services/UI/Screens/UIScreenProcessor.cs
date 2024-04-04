using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    public class UIScreenProcessor : UIProcessor
    {
        private Options _options;
        private UIScreen _openedScreen;
        private UIScreen.Args _openedScreenProperties;
        private Stack<UIScreen.Args> _history = new Stack<UIScreen.Args>();

        public Action<UIScreenType, UIScreenType> ScreenChanged;
        
        public UIScreenProcessor(Options options)
        {
            _options = options;
            OpenScreen(new UIScreen.Args(_options.StartScreen), true, true);
        }

        public void OpenScreen(UIScreen.Args args, bool pushToHistory = true, bool saveProperties = true)
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

                _openedScreen = GameObject.Instantiate(screenToOpen, _options.Holder);
                _openedScreen.Open(args);
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
        public class Options : UIProcessor.Options
        {
            [field: SerializeField] public UIScreenType StartScreen {get; private set;}
            [field: SerializeField] public UIScreen[] Screens {get; private set;}
        }
    }
}