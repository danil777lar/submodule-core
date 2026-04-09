using UnityEngine;
using ProjectConstants;

namespace Larje.Core.Services
{
    public abstract class IAPProductConfig : ScriptableObject
    {
        [SerializeField] private string productKey;
        [SerializeField] private string discountProductKey;
        [Space]
        [SerializeField] private int discountPercent;
        [Space]
        [SerializeField] private IAPProductType productType;
        [SerializeField] private IAPProductGroupType productGroupType;

        public string Id => name;

        public string ProductKey => productKey;
        public string DiscountProductKey => discountProductKey;

        public int DiscountPercent => discountPercent;

        public bool HasDiscountProduct => !string.IsNullOrEmpty(discountProductKey);

        public IAPProductType ProductType => productType;
        public IAPProductGroupType ProductGroupType => productGroupType;

        public abstract void ProductPurchaseComplete();
        public abstract void ProductPurchaseFailed();
    }
}
