using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIPopup : UIObject
    {
        [SerializeField] private UIPopupType _popupType;
        [Space]
        [SerializeField] private bool _closeByBackgroundClick;
        [Space]
        [SerializeField] private RectTransformEvents _background;
        private bool _isHidden;
        
        public UIPopupType PopupType => _popupType;

        public override void Open(UIObject.Args args)
        {
            if (!Opened)
            {
                if (_closeByBackgroundClick)
                {
                    _background.EventPointerClick += (x) => Close();
                }
            }
            
            base.Open(args);
        }
        
        public class Args : UIObject.Args
        {
            public readonly UIPopupType PopupType;
            public readonly UIPopupCombinationType CombinationType = UIPopupCombinationType.Close;

            public Args(UIPopupType popupType)
            {
                PopupType = popupType;
            }
            
            public Args(UIPopupType popupType, UIPopupCombinationType combinationType)
            {
                PopupType = popupType;
                CombinationType = combinationType;
            }
        }
    }
}