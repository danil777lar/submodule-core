using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Larje.Core.Services.UI
{
    [RequireComponent(typeof(Button))]
    public class UIBackButton : MonoBehaviour
    {
        [InjectService] private UIService _uiService;

        private Button _button;


        private void Start()
        {
            DIContainer.InjectTo(this);

            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked() 
        {
            _button.interactable = false;
            _uiService.Back();
        }
    }
}