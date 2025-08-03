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
        [InjectService] private IDataService _dataService;
        
        private List<CurrencyData> CurrencyData => _dataService.GameData.CurrencyData; 

        public event Action EventCurrencyChanged;
        
        public override void Init()
        {
        }

        public void AddCurrency(CurrencyType currency, CurrencyPlacementType placement, int count)
        {
            int currentCount = GetCurrency(currency, placement);
            SetCurrency(currency, placement, currentCount + count);
        }

        public void MoveCurrency(CurrencyType currency, CurrencyPlacementType placementFrom, CurrencyPlacementType placementTo)
        {
            int currentCount = GetCurrency(currency, placementFrom);
            SetCurrency(currency, placementFrom, 0);
            AddCurrency(currency, placementTo, currentCount);
        }

        public bool TrySpendCurrency(CurrencyType currency, CurrencyPlacementType placement, int count)
        {
            int currentCount = GetCurrency(currency, placement);
            if (currentCount < count)
            {
                return false;
            }
            else
            {
                SetCurrency(currency, placement, currentCount - count);
                return true;
            }
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
            _dataService.Save();
        }
    }
}