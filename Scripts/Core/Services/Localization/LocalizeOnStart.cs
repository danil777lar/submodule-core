using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using TMPro;
using UnityEngine;

namespace Larje.Core.Services
{
    public class LocalizeOnStart : MonoBehaviour
    {
        [SerializeField] private string key;
        [SerializeField] private string leftModifier;
        [SerializeField] private string rightModifier;

        [InjectService] private ILocalizationService _localizationService;

        private void Start()
        {
            DIContainer.InjectTo(this);
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                string localizedValue = _localizationService.GetLocalizationValue(key);
                text.text = $"{leftModifier}{localizedValue}{rightModifier}";
            }
        }
    }
}