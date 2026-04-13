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
    [BindService(typeof(IIAPService))]
    public class IAPService : Service, IIAPService, IDetailedStoreListener
    {
        [SerializeField] private List<IAPProductConfig> _products;

        [InjectService] private IDataService _dataService;

        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;

        private IAPServiceData Data => _dataService.GameData.IAPData;

        public override void Init()
        {
            InitializeServicesAndIAP();
        }

        public bool IsProductPurchased(IAPProductConfig config)
        {
            #if UNITY_EDITOR
            return Data.EditorPurchasedProductIds.Contains(config.Id);
            #endif

            if (_storeController == null)
            {
                return false;
            }

            Product product = _storeController.products.WithID(config.ProductKey);
            return product != null && product.hasReceipt;
        }

        public bool IsDiscountApplied(IAPProductConfig config)
        {
            return config.HasDiscountProduct && Data.IsDiscountApplied(config.Id);
        }

        public void ApplyDiscount(IAPProductConfig config)
        {
            ApplyDiscount(config, TimeSpan.Zero);
        }

        public void ApplyDiscount(IAPProductConfig config, TimeSpan duration)
        {
            if (!config.HasDiscountProduct)
            {
                Debug.LogWarning($"IAP Service: config '{config.Id}' has no discount product key");
                return;
            }

            Data.ApplyDiscount(config.Id, duration);
            _dataService.SaveGameData();
        }

        public void RemoveDiscount(IAPProductConfig config)
        {
            Data.RemoveDiscount(config.Id);
            _dataService.SaveGameData();
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _extensionProvider = extensions;
            Debug.Log("IAP Service: initialized successfully");
        }

        public IAPProductData GetProductData(IAPProductConfig config)
        {
            Product product = GetProduct(config);
            if (product == null)
            {
                return null;
            }

            return new IAPProductData(
                product.availableToPurchase,
                product.metadata.localizedPriceString,
                product.metadata.localizedTitle,
                product.metadata.localizedDescription,
                product.metadata.isoCurrencyCode,
                product.metadata.localizedPrice);
        }


        public List<IAPProductConfig> GetAllProducts()
        {
            return new List<IAPProductConfig>(_products);
        }

        public List<IAPProductConfig> GetProductsByGroup(IAPProductGroupType group)
        {
            return _products.FindAll(p => p.ProductGroupType == group);
        }

        public void BuyProduct(IAPProductConfig config)
        {
            string key = IsDiscountApplied(config) ? config.DiscountProductKey : config.ProductKey;
            BuyProduct(key);
        }

        public void BuyProduct(string productId)
        {
            #if UNITY_EDITOR
            if (!IsProductPurchased(GetProductConfig(productId)))
            {
                OnProductPurchased(GetProductConfig(productId));
            }
            else
            {
                Debug.LogWarning("IAP Service: product already purchased: " + productId);
            }
            return;
            #endif
            
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

            GetProductConfig(product.definition.id)?.ProductPurchaseFailed();
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
                    builder.AddProduct(config.ProductKey, ConvertProductType(config.ProductType));
                    if (config.HasDiscountProduct)
                    {
                        builder.AddProduct(config.DiscountProductKey, ConvertProductType(config.ProductType));
                    }
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
            Debug.Log("IAP Service: product purchased: " + config.Id);

            Data.EditorPurchasedProductIds.Add(config.Id);
            config.ProductPurchaseComplete();
        }

        private IAPProductConfig GetProductConfig(string key)
        {
            return _products.Find(p => p.ProductKey == key || p.DiscountProductKey == key);
        }

        private Product GetProduct(IAPProductConfig config)
        {
            if (_storeController == null)
            {
                Debug.LogError("IAP Service: cannot get product, not initialized");
                return null;
            }

            if (IsDiscountApplied(config))
            {
                Product discountProduct = _storeController.products.WithID(config.DiscountProductKey);
                if (discountProduct != null && discountProduct.availableToPurchase)
                {
                    return discountProduct;
                }
            }

            return _storeController.products.WithID(config.ProductKey);
        }

        private ProductType ConvertProductType(IAPProductType type)
        {
            switch (type)
            {
                case IAPProductType.Consumable:
                    return ProductType.Consumable;
                case IAPProductType.NonConsumable:
                    return ProductType.NonConsumable;
                case IAPProductType.Subscription:
                    return ProductType.Subscription;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported product type: {type}");
            }
        }
    }
}
#endif
