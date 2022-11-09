using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Larje.Core.Services.UI
{
    public class UIScreen : MonoBehaviour
    {
        [SerializeField] private UIScreenType _screenType;


        public UIScreenType ScreenType => _screenType;


        public UIScreen Open()
        {

            return this;
        }

        public void Close()
        {

        }
    }
}