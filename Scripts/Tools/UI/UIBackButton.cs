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

        private UIObject _uiObject;
        private Button _button;

        private void Start()
        {
            DIContainer.InjectTo(this);

            _uiObject = GetComponentInParent<UIObject>();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked() 
        {
            if (_uiService.FocusedObject.gameObject != _uiObject.gameObject)
            {
                return;
            }

            _button.interactable = false;
            _uiService.Back();
        }
    }
}
