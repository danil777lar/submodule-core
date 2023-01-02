using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using MoreMountains.Tools;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CurrencyCounter : MonoBehaviour
{
    [SerializeField] private CurrencyType _currencyType;
    [SerializeField] private bool specificPlacement; 
    [SerializeField, MMCondition("specificPlacement")] private CurrencyPlacementType _currencyPlacementType;
    [Space] 
    [SerializeField] private string _leftModificator;
    [SerializeField] private string _rightModificator = "<sprite index=0>";
        
    [InjectService] private CurrencyService _currencyService;
    private TextMeshProUGUI _tmp;
    
    private void Awake()
    {
        ServiceLocator.Default.InjectServicesInComponent(this);
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _currencyService.CurrencyChanged += OnCurrencyUpdated;
        OnCurrencyUpdated(_currencyType, _currencyPlacementType, _currencyService.GetCurrency(_currencyType, _currencyPlacementType));
    }
    
    private void OnDisable()
    {
        _currencyService.CurrencyChanged -= OnCurrencyUpdated;
    }

    private void OnCurrencyUpdated(CurrencyType currency, CurrencyPlacementType placement, int count)
    {
        if (!specificPlacement && currency == _currencyType)
        {
            int sum = 0;
            foreach (CurrencyPlacementType placeType in Enum.GetValues(typeof(CurrencyPlacementType)))
            {
                sum += _currencyService.GetCurrency(currency, placeType);
            }
            _tmp.text = $"{_leftModificator}{sum}{_rightModificator}";
        }
        else if (specificPlacement && currency == _currencyType && placement == _currencyPlacementType)
        {
            _tmp.text = $"{_leftModificator}{count}{_rightModificator}";
        }
    }
}
