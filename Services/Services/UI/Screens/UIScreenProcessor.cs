using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;
using UnityEngine.Serialization;

namespace Larje.Core.Services.UI
{
    public class UIScreenProcessor : UIProcessor
    {
        [Space]
        [SerializeField] private UIScreenType startScreen;
        [SerializeField] private UIScreen[] screens;
        
        private UIScreen _openedScreen;
        private UIScreen.Args _openedScreenProperties;
        private Stack<UIScreen.Args> _history = new Stack<UIScreen.Args>();

        public Action<UIScreenType, UIScreenType> ScreenChanged;
        
        public override void Init(int maxSortingOrder)
        {
            base.Init(maxSortingOrder);
            
            OpenScreen(new UIScreen.Args(startScreen));
        }

        public void OpenScreen(UIScreen.Args args, bool pushToHistory = true, bool saveProperties = true)
        {
            UIScreen screenToOpen = screens.First((screen) => screen.ScreenType == args.screenType);
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

                UIScreen screenInstance = Instantiate(screenToOpen, holder);
                screenInstance.Open(args);
                screenInstance.EventClose += () => OnScreenClosed(screenInstance);
                _openedScreen = screenInstance;
                
                AddOpenedUIObject(screenInstance);

                if (screenToClose)
                {
                    ScreenChanged?.Invoke(screenToClose.ScreenType, args.screenType);
                    screenToClose.Close();
                }
            }
        }

        public override bool ComputeDeviceBackButton()
        {
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

        private void OnScreenClosed(UIScreen closedScreen)
        {
            RemoveOpenedUIObject(closedScreen);   
        }
    }
}