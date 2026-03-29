using System;
using ProjectConstants;

namespace Larje.Core.Services
{
    public interface ICurrencyService
    {
        public event Action EventCurrencyChanged;
        public event Action<CurrencyOperationData> EventCurrencyAdded;
        public event Action<CurrencyOperationData> EventCurrencySpent;

        public void AddCurrency(CurrencyOperationData data);

        public bool TrySpendCurrency(CurrencyOperationData data);

        public bool CheckEnoughCurrency(CurrencyType currency, CurrencyPlacementType placement, int count);

        public void SetCurrency(CurrencyType currency, CurrencyPlacementType placement, int count);

        public void MoveCurrency(CurrencyType currency, CurrencyPlacementType placementFrom, CurrencyPlacementType placementTo);

        public int GetCurrency(CurrencyType currency, CurrencyPlacementType placement);
    }
}
