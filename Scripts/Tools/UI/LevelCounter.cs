using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LevelCounter : MonoBehaviour
{
    [SerializeField] private int addToCount;
    [Space]
    [SerializeField] private string leftPrefix;
    [SerializeField] private string rightPrefix;

    [InjectService] private ILocalizationService _localizationService;
    [InjectService] private ILevelManagerService _levelService;
    
    private void Start()
    {
        DIContainer.InjectTo(this);

        TextMeshProUGUI tmp = GetComponent<TextMeshProUGUI>();
        int level = _levelService.GetCurrentLevelCount() + addToCount;

        string levelStr = $"{GetPrefix(leftPrefix)}{level}{GetPrefix(rightPrefix)}";
        tmp.text = levelStr;
    }

    private string GetPrefix(string rawPrefix)
    {
        return string.IsNullOrEmpty(rawPrefix) ? "" : $"{_localizationService.GetLocalizationValue(rawPrefix)}";
    }
}
