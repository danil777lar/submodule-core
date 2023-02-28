using ProjectConstants;

namespace Larje.Core.Services
{
    public interface ICurrencyService
    {
        public void AddCurrency(CurrencyType currency, CurrencyPlacementType placement, int count);

        public void MoveCurrency(CurrencyType currency, CurrencyPlacementType placementFrom, CurrencyPlacementType placementTo);

        public bool TrySpendCurrency(CurrencyType currency, CurrencyPlacementType placement, int count);

        public bool CheckEnoughCurrency(CurrencyType currency, CurrencyPlacementType placement, int count);

        public int GetCurrency(CurrencyType currency, CurrencyPlacementType placement);

        public void SetCurrency(CurrencyType currency, CurrencyPlacementType placement, int count);
    }
}