using System;
using System.Collections.Generic;
using System.Linq;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    [BindService(typeof(UIService))]
    public class UIService : Service
    {
        [SerializeField] private bool useDeviceBackButton = true;
        [SerializeField] private int maxSortOrder;

        private List<UIProcessor> _processors;
        
        public override void Init()
        {
            _processors = new List<UIProcessor>(GetComponentsInChildren<UIProcessor>());
            _processors.ForEach(x =>
            {
                x.Init(maxSortOrder);
                x.EventOpenedObjectsChanged += OnProcessorOpenedObjectsChanged;
            });
        }

        public void Back()
        {
            foreach (UIProcessor processor in GetProcessorsByPriority())
            {
                if (processor.Back())
                {
                    return;
                }
            }
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
                    Back();
                }
            }
        }
        
        private void OnProcessorOpenedObjectsChanged()
        {
            int offset = 0;
            foreach (UIProcessor processor in GetProcessorsByPriority())
            {
                offset += processor.SetSortingOrders(offset);
            }
        }
        
        private List<UIProcessor> GetProcessorsByPriority()
        {
            return _processors.OrderBy(x => x.Priority).Reverse().ToList();
        }
        
        [ContextMenu("Show Test Toast")]
        private void ShowTestToast()
        {
            GetProcessor<UIToastProcessor>().OpenToast(new UIToast.Args(UIToastType.Info, "Test toast message"));
        }
    }
}