using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Tools;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    [BindService(typeof(ICurrencyService))]
    public class CurrencyService : Service, ICurrencyService
    {
        [InjectService] private DataService _dataService;
        private List<CurrencyData> _currencyDatas;

        public event Action CurrencyChanged;
        
        public override void Init()
        {
            _currencyDatas = _dataService.Data.CurrencyData;
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
            return currentCount <= count;
        }

        public int GetCurrency(CurrencyType currency, CurrencyPlacementType placement)
        {
            CurrencyData currencyData = _currencyDatas.Find(x => x.CurrencyType == currency);
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
            CurrencyData currencyData = _currencyDatas.Find(x => x.CurrencyType == currency);
            if (currencyData == null)
            {
                currencyData = new CurrencyData().Build(currency);
                _currencyDatas.Add(currencyData);
            }

            PlacementData placementData = currencyData.placements.Find(x => x.CurrencyPlacementType == placement);
            if (placementData == null)
            {
                placementData = new PlacementData().Build(placement);
                currencyData.placements.Add(placementData);
            }

            placementData.count = count;
            CurrencyChanged?.Invoke();
            _dataService.Save();
        }
    }
}