using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Larje.Core.Tools
{
    public class RewardedAdButton : MonoBehaviour
    {
        [SerializeField] private GameObject adNotAvailableLocker;
        
        [InjectService] private IAdsService _adsService;
        
        private bool _enableChecking = true; 
        private Button _button;

        public void SetEnableChecking(bool arg)
        {
            _enableChecking = arg;
            if (!_enableChecking)
            {
                adNotAvailableLocker.SetActive(false);
                if (_button != null)
                {
                    _button.interactable = true;
                }
            }
        }
        
        private void Start()
        {
            DIContainer.InjectTo(this);
            _button = GetComponent<Button>();
        }

        private void Update()
        {
            if (_enableChecking)
            {
                if (adNotAvailableLocker != null)
                {
                    adNotAvailableLocker.SetActive(!_adsService.RewardedAdAvailable);
                }

                _button.interactable = _adsService.RewardedAdAvailable;
            }
        }
    }
}