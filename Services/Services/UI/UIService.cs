using System;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    [BindService(typeof(UIService))]
    public class UIService : Service
    {
        [SerializeField] private bool _useDeviceBackButton = true;
        [Space]
        [SerializeField] private UIScreenProcessor.Options _screenProcessorOptions;
        [SerializeField] private UIPopupProcessor.Options _popupProcessorOptions;
        [SerializeField] private UIToastProcessor.Options _toastProcessorOptions;

        private List<UIProcessor> _processors = new List<UIProcessor>();
        
        public UIScreenProcessor Screens { get; private set; }
        public UIPopupProcessor Popups { get; private set; }
        public UIToastProcessor Toasts { get; private set; }
        
        private void Update()
        {
            UpdateDeviceBackButton();
        }

        public override void Init() 
        {
            Screens = new UIScreenProcessor(_screenProcessorOptions);
            _processors.Add(Screens);
            
            Popups = new UIPopupProcessor(_popupProcessorOptions);
            _processors.Add(Popups);
            
            Toasts = new UIToastProcessor(_toastProcessorOptions);
            _processors.Add(Toasts);
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