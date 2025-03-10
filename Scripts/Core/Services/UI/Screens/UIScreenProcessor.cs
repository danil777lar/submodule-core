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
        [SerializeField] private bool spawnScreenOnStart;
        [SerializeField] private UIScreenType startScreen;
        [Space]
        [SerializeField] private UIScreen[] screens;

        private List<UIScreen> _openedScreens = new List<UIScreen>();

        public UIScreenType LastOpenedScreen { get; private set; }

        public event Action<UIScreenType> EventScreenOpened;
        public event Action<UIScreenType> EventScreenClosed;
        
        public override void Init(int maxSortingOrder)
        {
            base.Init(maxSortingOrder);

            if (spawnScreenOnStart)
            {
                OpenScreen(new UIScreen.Args(startScreen));
            }
        }

        public void OpenScreen(UIScreen.Args args)
        {
            UIScreen screenToOpen = screens.First((screen) => screen.ScreenType == args.screenType);
            if (screenToOpen != null)
            {
                LastOpenedScreen = args.screenType;
                
                if (screenToOpen.ClearHistoryOnOpen)
                {
                    _openedScreens.ForEach(x => x.Close());
                }
                else if (_openedScreens.Count > 0)
                {
                    _openedScreens.Last().Hide();
                }

                UIScreen screenInstance = Instantiate(screenToOpen, holder);
                screenInstance.Open(args);
                screenInstance.EventAfterClose += () => OnScreenClosed(screenInstance);
                _openedScreens.Add(screenInstance);
                EventScreenOpened?.Invoke(screenInstance.ScreenType);
                
                AddOpenedUIObject(screenInstance);
            }
        }

        public override bool Back()
        {
            for (int i = _openedScreens.Count - 1; i >= 0; i--)
            {
                bool result = _openedScreens[i].Back(i == 0);
                if (result)
                {
                    if (i > 0)
                    {
                        _openedScreens[i - 1].Show();
                    }

                    return true;
                }
            }

            return false;
        }

        private void OnScreenClosed(UIScreen closedScreen)
        {
            _openedScreens.Remove(closedScreen);
            RemoveOpenedUIObject(closedScreen);   
            EventScreenClosed?.Invoke(closedScreen.ScreenType);
        }
    }
}