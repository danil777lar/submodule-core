#if IAP_ENABLED
using UnityEngine;
using UnityEngine.Purchasing;
using ProjectConstants;

namespace Larje.Core.Services
{
    public abstract class IAPProductConfig : ScriptableObject
    {
        [SerializeField] private string productKey;
        [SerializeField] private ProductType productType;
        [SerializeField] private IAPProductGroupType productGroupType;

        public string ProductKey => productKey;
        public ProductType ProductType => productType;
        public IAPProductGroupType ProductGroupType => productGroupType;

        public abstract void ProductPurchaseComplete();
        public abstract void ProductPurchaseFailed(PurchaseFailureDescription failureDescription);
    }
}
#endif
