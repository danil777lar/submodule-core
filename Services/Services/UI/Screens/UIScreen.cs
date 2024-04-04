using System;
using System.Linq;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIScreen : UIObject
    {
        [field: SerializeField] public UIScreenType ScreenType { get; private set; }
        [field: SerializeField] public bool RewriteDeviceBackButton { get; private set; }

        protected virtual void OnDeviceBackButton()
        {
            
        }

        private void Update()
        {
            if (RewriteDeviceBackButton && Input.GetKeyDown(KeyCode.Escape))
            {
                OnDeviceBackButton();
            }
        }

        public class Args : UIObject.Args
        {
            public readonly UIScreenType screenType;

            public Args(UIScreenType screenType)
            {
                this.screenType = screenType;
            }
        }
    }
}