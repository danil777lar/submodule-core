using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Tools;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    [BindService(typeof(ICurrencyService), typeof(CurrencyService))]
    public class CurrencyService : Service, ICurrencyService
    {
        [SerializeField] private List<CurrencyOptions> options = new();

        [InjectService] private IDataService _dataService;

        private List<CurrencyData> CurrencyData => _dataService.GameData.CurrencyData;

        public event Action EventCurrencyChanged;
        public event Action<CurrencyOperationData> EventCurrencyAdded;
        public event Action<CurrencyOperationData> EventCurrencySpent;

        public override void Init()
        {
            foreach (CurrencyOptions option in options)
            {
                CurrencyData currencyData = CurrencyData.Find(x => x.CurrencyType == option.Currency);
                if (currencyData != null && currencyData.defaultsApplied)
                {
                    continue;
                }

                if (option.DefaultAmount > 0)
                {
                    SetCurrency(option.Currency, CurrencyPlacementType.Global, option.DefaultAmount);
                }

                currencyData = CurrencyData.Find(x => x.CurrencyType == option.Currency);
                if (currencyData == null)
                {
                    currencyData = new CurrencyData().Build(option.Currency);
                    CurrencyData.Add(currencyData);
                }

                currencyData.defaultsApplied = true;
                _dataService.SaveGameData();
            }
        }

        public void AddCurrency(CurrencyOperationData data)
        {
            int currentCount = GetCurrency(data.Currency, data.Placement);
            SetCurrency(data.Currency, data.Placement, currentCount + data.Amount);
            EventCurrencyAdded?.Invoke(data);
        }

        public bool TrySpendCurrency(CurrencyOperationData data)
        {
            int currentCount = GetCurrency(data.Currency, data.Placement);
            if (currentCount < data.Amount)
            {
                return false;
            }
            else
            {
                SetCurrency(data.Currency, data.Placement, currentCount - data.Amount);
                EventCurrencySpent?.Invoke(data);
                return true;
            }
        }

        public void MoveCurrency(CurrencyType currency, CurrencyPlacementType placementFrom, CurrencyPlacementType placementTo)
        {
            int currentCount = GetCurrency(currency, placementFrom);
            SetCurrency(currency, placementFrom, 0);
            AddCurrency(new CurrencyOperationData{ Currency = currency, Placement = placementTo, Amount = currentCount });
        }

        public bool CheckEnoughCurrency(CurrencyType currency, CurrencyPlacementType placement, int count)
        {
            int currentCount = GetCurrency(currency, placement);
            return currentCount >= count;
        }

        public int GetCurrency(CurrencyType currency, CurrencyPlacementType placement)
        {
            CurrencyData currencyData = CurrencyData.Find(x => x.CurrencyType == currency);
            if (currencyData == null)
            {
                return 0;
            }

            PlacementData placementData = currencyData.placements.Find(x => x.CurrencyPlacementType == placement);
            if (placementData == null)
            {
                return 0;
            }

            return placementData.count;
        }

        public void SetCurrency(CurrencyType currency, CurrencyPlacementType placement, int count)
        {
            CurrencyOptions currencyOptions = options.Find(x => x.Currency == currency);
            if (currencyOptions != null && currencyOptions.MaxAmount > 0)
            {
                count = Mathf.Clamp(count, 0, currencyOptions.MaxAmount);
            }

            CurrencyData currencyData = CurrencyData.Find(x => x.CurrencyType == currency);
            if (currencyData == null)
            {
                currencyData = new CurrencyData().Build(currency);
                CurrencyData.Add(currencyData);
            }

            PlacementData placementData = currencyData.placements.Find(x => x.CurrencyPlacementType == placement);
            if (placementData == null)
            {
                placementData = new PlacementData().Build(placement);
                currencyData.placements.Add(placementData);
            }

            placementData.count = count;
            EventCurrencyChanged?.Invoke();
            _dataService.SaveGameData();
        }

        [Serializable]
        public class CurrencyOptions
        {
            public CurrencyType Currency;
            public int DefaultAmount;
            public int MaxAmount;
        }
    }
}
