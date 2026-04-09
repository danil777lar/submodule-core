using System;
using System.Collections.Generic;
using ProjectConstants;

namespace Larje.Core.Services
{
    public interface IIAPService
    {
        bool IsProductPurchased(IAPProductConfig config);
        bool IsDiscountApplied(IAPProductConfig config);
        void ApplyDiscount(IAPProductConfig config);
        void ApplyDiscount(IAPProductConfig config, TimeSpan duration);
        void RemoveDiscount(IAPProductConfig config);

        IAPProductData GetProductData(IAPProductConfig config);
        List<IAPProductConfig> GetAllProducts();
        List<IAPProductConfig> GetProductsByGroup(IAPProductGroupType group);

        void BuyProduct(IAPProductConfig config);
        void BuyProduct(string productId);

        void RestorePurchases();
    }
}
