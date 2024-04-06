using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    public abstract class UIProcessor : MonoBehaviour
    {
        [field: SerializeField] public int Priority { get; private set; }
        
        protected Transform holder;
        
        private int _maxSortingOrder;
        private List<UIObject> _openedUIObjects = new List<UIObject>();
        
        public event Action EventOpenedObjectsChanged;
        public event Action EventShownObjectsChanged;
        
        public virtual void Init(int maxSortingOrder)
        {
            _maxSortingOrder = maxSortingOrder;
            
            holder = new GameObject($"{GetType().Name} Holder").transform;
            holder.SetParent(transform);
            holder.localPosition = Vector3.zero;
            holder.localScale = Vector3.one;
        }
        
        public int SetSortingOrders(int processorOffset)
        {
            int orderOffset = 0;
            foreach (UIObject uiObject in _openedUIObjects.ToArray().Reverse())
            {
                if (uiObject != null)
                {
                    orderOffset++;
                    int order = _maxSortingOrder - orderOffset - processorOffset;
                    uiObject.SetSortingOrder(order);
                }
            }
            
            return _openedUIObjects.Count;
        }

        public GameObject SetFocusStates(bool canBeFocused)
        {
            GameObject focusedObject = null; 
            foreach (UIObject uiObject in _openedUIObjects.ToArray().Reverse())
            {
                if (uiObject != null && uiObject.FocusTarget)
                {
                    if (focusedObject != null || !canBeFocused)
                    {
                        uiObject.Unfocus();
                    }
                    else
                    {
                        uiObject.Focus();
                        focusedObject = uiObject.gameObject;   
                    }
                }
            }
            
            return focusedObject;
        }
        
        public abstract bool Back();
        
        protected void AddOpenedUIObject(UIObject uiObject)
        {
            if (!_openedUIObjects.Contains(uiObject))
            {
                _openedUIObjects.Add(uiObject);
                
                uiObject.EventShow += OnUiObjectShowHide;
                uiObject.EventHide += OnUiObjectShowHide;
                
                EventOpenedObjectsChanged?.Invoke();
                EventShownObjectsChanged?.Invoke();
            }
        }
        
        protected void RemoveOpenedUIObject(UIObject uiObject)
        {
            if (_openedUIObjects.Contains(uiObject))
            {
                _openedUIObjects.Remove(uiObject);
                EventOpenedObjectsChanged?.Invoke();
                EventShownObjectsChanged?.Invoke();
            }
        }
        
        private void OnUiObjectShowHide()
        {
            EventShownObjectsChanged?.Invoke();
        }
    }
}