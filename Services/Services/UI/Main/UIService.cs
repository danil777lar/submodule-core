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
        private ScreenOpenProperties _openedScreenProperties;
        private Stack<ScreenOpenProperties> _history;


        private void Awake()
        {
            _history = new Stack<ScreenOpenProperties>();
            ShowScreen(new ScreenOpenProperties(_defaultScreenType, false));
        }

        public void ShowScreen(ScreenOpenProperties args, bool pushToHistory = true) 
        {
            UIScreen screenToOpen = _screens.Find((screen) => screen.ScreenType == args.screenType);
            if (screenToOpen != null)
            {
                _openedScreen?.Close();
                if (_openedScreenProperties != null && pushToHistory) 
                {
                    _history.Push(_openedScreenProperties); 
                }
                _openedScreenProperties = args;
                _openedScreen = Instantiate(screenToOpen, _screenHolder).Open(args.screenArguments);
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
            public readonly bool withAnim;
            public readonly object[] screenArguments;

            public ScreenOpenProperties(UIScreenType screenType, bool withAnim = true, object[] screenArguments = null) 
            {
                this.screenType = screenType;
                this.withAnim = withAnim;
                this.screenArguments = screenArguments;
            }
        }
    }
}