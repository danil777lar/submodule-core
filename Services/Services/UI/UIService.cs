using System;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    [BindService(typeof(UIService))]
    public class UIService : Service
    {
        [SerializeField] private bool useDeviceBackButton = true;
        [SerializeField] private int minSortOrder;

        private List<UIProcessor> _processors;
        
        public override void Init()
        {
            _processors = new List<UIProcessor>(GetComponentsInChildren<UIProcessor>());
            _processors.ForEach(x => x.Init());
        }
        
        public T GetProcessor<T>() where T : UIProcessor
        {
            return _processors.Find((processor) => processor is T) as T;
        }
        
        private void Update()
        {
            UpdateDeviceBackButton();
        }

        private void UpdateDeviceBackButton() 
        {
            if (useDeviceBackButton)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                }
            }
        }
    }
}