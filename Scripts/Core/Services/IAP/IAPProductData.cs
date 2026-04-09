namespace Larje.Core.Services
{
    public class IAPProductData
    {
        public bool AvailableToPurchase { get; }
        public string LocalizedPriceString { get; }
        public string LocalizedTitle { get; }
        public string LocalizedDescription { get; }
        public string IsoCurrencyCode { get; }
        public decimal LocalizedPrice { get; }

        public IAPProductData(
            bool availableToPurchase,
            string localizedPriceString,
            string localizedTitle,
            string localizedDescription,
            string isoCurrencyCode,
            decimal localizedPrice)
        {
            AvailableToPurchase = availableToPurchase;
            LocalizedPriceString = localizedPriceString;
            LocalizedTitle = localizedTitle;
            LocalizedDescription = localizedDescription;
            IsoCurrencyCode = isoCurrencyCode;
            LocalizedPrice = localizedPrice;
        }
    }
}
