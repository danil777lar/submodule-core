using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    [BindService(typeof(UIService))]
    public class UIService : Service
    {
        [SerializeField] private bool _useDeviceBackButton = true;
        [SerializeField] private bool _saveFirstScreen = false;
        [Space]
        [SerializeField] private UIScreenType _defaultScreenType;
        [Space]
        [SerializeField] private UIScreenProcessor.Options _screenProcessorOptions;
        [SerializeField] private UIPopupProcessor.Options _popupProcessorOptions;
        [SerializeField] private UIToastProcessor.Options _toastProcessorOptions;

        public UIScreenProcessor Screens 
        {
            get;
            private set;
        }
        public UIPopupProcessor Popups
        {
            get;
            private set;
        }
        public UIToastProcessor Toasts
        {
            get;
            private set;
        }

        public Action<UIScreenType, UIScreenType> ScreenChanged => Screens.ScreenChanged;


        private void Update()
        {
            UpdateDeviceBackButton();
        }

        public override void Init() 
        {
            Screens = new UIScreenProcessor(_screenProcessorOptions);
            Popups = new UIPopupProcessor(_popupProcessorOptions);
            Toasts = new UIToastProcessor(_toastProcessorOptions);

            Screens.OpenScreen(new ScreenOpenProperties(_defaultScreenType), true, _saveFirstScreen);
        }


        private void UpdateDeviceBackButton() 
        {
            if (_useDeviceBackButton)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (!Popups.TryClosePopupByBackButton())
                    {
                        Screens.ComputeDeviceBackButton();
                    }
                }
            }
        }
    }
}