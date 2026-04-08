#if IAP_ENABLED
using System;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using ProjectConstants;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Larje.Core.Services
{
    [BindService(typeof(IAPService))]
    public class IAPService : Service, IDetailedStoreListener
    {
        [SerializeField] private List<IAPProductConfig> _products;

        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;

        public override void Init()
        {
            InitializeServicesAndIAP();
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _extensionProvider = extensions;
            Debug.Log("IAP Service: initialized successfully");
        }

        public Product GetProduct(IAPProductConfig config)
        {
            if (_storeController == null)
            {
                Debug.LogError("IAP Service: cannot get product, not initialized");
                return null;
            }

            return _storeController.products.WithID(config.ProductKey);
        }

        public List<IAPProductConfig> GetProductsByGroup(IAPProductGroupType group)
        {
            return _products.FindAll(p => p.ProductGroupType == group);
        }

        public void BuyProduct(IAPProductConfig config)
        {
            BuyProduct(config.ProductKey);
        }

        public void BuyProduct(string productId)
        {
            if (_storeController == null)
            {
                Debug.LogError("IAP Service: StoreController is null");
                return;
            }

            Product product = _storeController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                _storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.LogError("IAP Service: product not found or unavailable: " + productId);
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError("IAP Service: initialize failed: " + error);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"IAP Service: initialize failed: {error}, {message}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            IAPProductConfig config = GetProductConfig(purchaseEvent.purchasedProduct.definition.id);
            if (config != null)
            {
                OnProductPurchased(config);
            }
            else
            {
                Debug.LogWarning("IAP Service: purchased unknown product: " + purchaseEvent.purchasedProduct.definition.id);
            }

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.LogError($"IAP Service: purchase failed: {product.definition.id}, reason: {failureDescription.reason}, message: {failureDescription.message}");

            GetProductConfig(product.definition.id)?.ProductPurchaseFailed(failureDescription);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
        }

        public void RestorePurchases()
        {
            if (_extensionProvider == null)
            {
                Debug.LogError("IAP Service: cannot restore, not initialized");
                return;
            }

    #if UNITY_IOS
            _extensionProvider.GetExtension<IAppleExtensions>()
                .RestoreTransactions((result, error) =>
                {
                    if (result)
                        Debug.Log("IAP Service: restore successful");
                    else
                        Debug.LogError("IAP Service: restore failed: " + error);
                });
    #elif UNITY_ANDROID
            _extensionProvider.GetExtension<IGooglePlayStoreExtensions>()
                .RestoreTransactions((result, error) =>
                {
                    if (result)
                        Debug.Log("IAP Service: restore successful");
                    else
                        Debug.LogError("IAP Service: restore failed: " + error);
                });
    #endif
        }

        private async System.Threading.Tasks.Task InitializeServicesAndIAP()
        {
            try
            {
                await UnityServices.InitializeAsync();

                ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
                foreach (IAPProductConfig config in _products)
                {
                    builder.AddProduct(config.ProductKey, config.ProductType);
                }

                UnityPurchasing.Initialize(this, builder);
            }
            catch (Exception e)
            {
                Debug.LogError("IAP Service: init failed: " + e.Message);
            }
        }

        private void OnProductPurchased(IAPProductConfig config)
        {
            config.ProductPurchaseComplete();
        }

        private IAPProductConfig GetProductConfig(string key)
        {
            return _products.Find(p => p.ProductKey == key);
        }
    }
}
#endif
