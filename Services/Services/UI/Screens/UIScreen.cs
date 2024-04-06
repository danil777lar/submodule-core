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
        [field: SerializeField] public bool ClearHistoryOnOpen { get; private set; }
        [field: SerializeField] public UIScreenType ScreenType { get; private set; }

        public new class Args : UIObject.Args
        {
            public readonly UIScreenType screenType;

            public Args(UIScreenType screenType)
            {
                this.screenType = screenType;
            }
        }
    }
}