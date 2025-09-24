using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
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
        DIContainer.InjectTo(this);
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _currencyService.EventCurrencyChanged += OnEventCurrencyUpdated;
        OnEventCurrencyUpdated();
    }
    
    private void OnDisable()
    {
        _currencyService.EventCurrencyChanged -= OnEventCurrencyUpdated;
    }

    private void OnEventCurrencyUpdated()
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
