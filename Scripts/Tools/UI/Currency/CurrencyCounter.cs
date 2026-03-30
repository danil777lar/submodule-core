using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using ProjectConstants;
using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CurrencyCounter : MonoBehaviour
{
    [Space]
    [SerializeField] private bool autoAddUpdate = true;
    [SerializeField] private bool autoSpendUpdate = true;

    [Header("Main")]
    [SerializeField] private CurrencyType currencyType;
    [SerializeField] private CurrencyPlacementTypes currencyPlacementTypes;

    [Header("Modificators")]
    [SerializeField] private string leftModificator;
    [SerializeField] private string rightModificator = "<sprite index=0>";
        
    [InjectService] private ICurrencyService _currencyService;
    
    private TextMeshProUGUI _tmp;

    public CurrencyType CurrencyType => currencyType;

    public void SetValue(int value)
    {
        _tmp.text = $"{leftModificator}{value}{rightModificator}";
    }

    public int GetCurrentValue()
    {
        int sum = 0;
        foreach (CurrencyPlacementType placement in Enum.GetValues(typeof(CurrencyPlacementType)))
        {
            if (currencyPlacementTypes.HasFlag((CurrencyPlacementTypes)(int)placement))
            {
                sum += _currencyService.GetCurrency(currencyType, placement);    
            }
        }

        return sum;
    }

    public void PlayUpdateAnim()
    {
        this.DOKill();
        Sequence seq = DOTween.Sequence().SetTarget(this);

        seq.Append(transform.DOScale(0.5f, 0.1f).SetTarget(this));
        seq.Append(transform.DOScale(1f, 0.2f).SetTarget(this).SetEase(Ease.OutBack));
    }
    
    private void Awake()
    {
        DIContainer.InjectTo(this);
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _currencyService.EventCurrencyAdded += OnCurrencyAdded;
        _currencyService.EventCurrencySpent += OnCurrencySpent;

        OnEventCurrencyUpdated();
    }
    
    private void OnDisable()
    {
        _currencyService.EventCurrencyAdded -= OnCurrencyAdded;
        _currencyService.EventCurrencySpent -= OnCurrencySpent;
    }

    private void OnCurrencyAdded(CurrencyOperationData data)
    {
        if (data.Currency == currencyType && autoAddUpdate)
        {
            OnEventCurrencyUpdated();
        }
    }

    private void OnCurrencySpent(CurrencyOperationData data)
    {
        if (data.Currency == currencyType && autoSpendUpdate)
        {
            OnEventCurrencyUpdated();
        }
    }

    private void OnEventCurrencyUpdated()
    {
        SetValue(GetCurrentValue());
    }
}
