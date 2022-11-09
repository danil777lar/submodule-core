using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Larje.Core.Services.UI
{
    public class UIScreen : MonoBehaviour
    {
        [SerializeField] private UIScreenType _screenType;

        public Action ScreenOpen;
        public Action ScreenClose;
        public UIScreenType ScreenType => _screenType;


        public UIScreen Open()
        {
            ScreenOpen?.Invoke();
            return this;
        }

        public void Close()
        {
            ScreenClose?.Invoke();
            float delay = GetComponentsInChildren<IUIScreenCloseDelay>().Max((delay) => delay.GetDelay());
            Destroy(gameObject, delay);
        }
    }
}