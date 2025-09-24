using System;
using System.Collections.Generic;
using System.Linq;
using ProjectConstants;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Larje.Core.Services.UI
{
    [BindService(typeof(UIService))]
    public class UIService : Service
    {
        [SerializeField] private bool useDeviceBackButton = true;
        [SerializeField] private int maxSortOrder;
        [field: Space]
        [field: SerializeField] public GameObject FocusedObject { get; private set; }
        
        [InjectService] private InputService inputService;
        
        private List<UIProcessor> _processors;
        
        public override void Init()
        {
            _processors = new List<UIProcessor>(GetComponentsInChildren<UIProcessor>());
            _processors.ForEach(x =>
            {
                x.Init(maxSortOrder);
                x.EventOpenedObjectsChanged += OnProcessorOpenedObjectsChanged;
                x.EventShownObjectsChanged += OnProcessorShownObjectsChanged;
            });
        }

        public void SetWorldCamera(Camera canvasCamera)
        {
            _processors.ForEach(x => x.SetWorldCamera(canvasCamera));
        }

        public void SetVirtualScreenHitProcessor(Func<PointerEventData, VirtualScreen.VirtualScreenHit> virtualScreenHitProcessor)
        {
            _processors.ForEach(x => x.SetVirtualScreenHitProcessor(virtualScreenHitProcessor));
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
                if (inputService.UIBack.WasPerformedThisFrame())
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
        
        private void OnProcessorShownObjectsChanged()
        {
            GameObject focusedObject = null;
            foreach (UIProcessor processor in GetProcessorsByPriority())
            {
                GameObject focused = processor.SetFocusStates(focusedObject == null);
                if (focused != null)
                {
                    focusedObject = focused;
                }
            }

            FocusedObject = focusedObject;
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