using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using MoreMountains.Tools;
using ProjectConstants;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CurrencyCounter : MonoBehaviour
{
    [SerializeField] private CurrencyType currencyType;
    [SerializeField] private CurrencyPlacementTypes currencyPlacementTypes;
    [Space] 
    [SerializeField] private string leftModificator;
    [SerializeField] private string rightModificator = "<sprite index=0>";
        
    [InjectService] private ICurrencyService _currencyService;
    
    private TextMeshProUGUI _tmp;
    
    private void Awake()
    {
        ServiceLocator.Default.InjectServicesInComponent(this);
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _currencyService.CurrencyChanged += OnCurrencyUpdated;
        OnCurrencyUpdated();
    }
    
    private void OnDisable()
    {
        _currencyService.CurrencyChanged -= OnCurrencyUpdated;
    }

    private void OnCurrencyUpdated()
    {
        int sum = 0;
        foreach (CurrencyPlacementType placement in Enum.GetValues(typeof(CurrencyPlacementType)))
        {
            if (currencyPlacementTypes.HasFlag((CurrencyPlacementTypes)(int)placement))
            {
                sum += _currencyService.GetCurrency(currencyType, placement);    
            }
        }
        _tmp.text = $"{leftModificator}{sum}{rightModificator}";
    }
}
