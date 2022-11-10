using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Larje.Core.Services.UI
{
    [RequireComponent(typeof(Button))]
    public class PreviousScreenButton : MonoBehaviour
    {
        private void Start()
        {
            UIService uiService = ServiceLocator.Default.GetService<UIService>();
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() => 
            {
                button.interactable = false;
                uiService.ShowPreviousScreen();
            });
        }
    }
}