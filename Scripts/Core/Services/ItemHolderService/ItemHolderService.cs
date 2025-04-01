using System;
using System.Collections.Generic;
using System.Linq;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    [BindService(typeof(IItemHolderService), typeof(ItemHolderService))]
    public class ItemHolderService : Service, IItemHolderService
    {
        [SerializeField] private bool setFirstItemAsDefault;
        [SerializeField] private List<ItemsHolderConfig> configs;

        [InjectService] private DataService _dataService;

        public event Action EventCurrentItemChanged;
        public event Action EventNewItemUnlocked;

        public override void Init()
        {
            DIContainer.InjectTo(this, typeof(ItemHolderService));
        }

        public void UnlockItem(ItemType itemType, string itemName)
        {
            GetSkinsData(itemType).UnlockedItems.Add(itemName);
            EventNewItemUnlocked?.Invoke();
        }

        public void SetCurrentItem(ItemType itemType, string itemName)
        {
            bool canSet = string.IsNullOrEmpty(itemName) || string.IsNullOrWhiteSpace(itemName);
            canSet |= IsItemUnlocked(itemType, itemName) &&
                      (!TryGetCurrentItem(out Item currentItem, itemType) || currentItem.Name != itemName);  
            
            if (canSet)
            {
                GetSkinsData(itemType).CurrentItem = itemName;
                EventCurrentItemChanged?.Invoke();    
            }
        }

        public bool IsItemUnlocked(ItemType itemType, string itemName)
        {
            return GetSkinsData(itemType).UnlockedItems.Contains(itemName);
        }

        public bool TryGetCurrentItem(out Item currentItem, ItemType itemType)
        {
            currentItem = GetConfigByType(itemType).GetItem(GetSkinsData(itemType).CurrentItem);
            return currentItem != null;
        }

        public bool HasLockedItems(ItemType itemType)
        {
            bool hasLockedItems = false;
            foreach (Item item in GetAllItems(itemType))
            {
                hasLockedItems |= !IsItemUnlocked(itemType, item.Name);
            }
            return hasLockedItems;
        }

        public List<Item> GetAllItems(ItemType itemType)
        {
            return GetConfigByType(itemType).Items.ToList();
        }
        public List<Item> GetLockedItems(ItemType itemType)
        {
            return GetConfigByType(itemType).Items.ToList().FindAll(x => !IsItemUnlocked(itemType, x.Name));
        }
        
        public List<Item> GetUnlockedItems(ItemType itemType)
        {
            return GetConfigByType(itemType).Items.ToList().FindAll(x => IsItemUnlocked(itemType, x.Name));
        }

        protected ItemsData GetSkinsData(ItemType itemType)
        {
            ItemsData data = _dataService.Data.ItemsData
                .ToList().Find(x => x.ItemType == itemType);

            if (data == null)
            {
                string defaultSkin = setFirstItemAsDefault ? GetConfigByType(itemType).Items[0].Name : " ";

                data = new ItemsData();
                data.ItemType = itemType;
                data.CurrentItem = defaultSkin;
                data.UnlockedItems.Add(defaultSkin);
                _dataService.Data.ItemsData.Add(data);
                _dataService.Save();
            }

            return data;
        }

        protected ItemsHolderConfig GetConfigByType(ItemType itemType)
        {
            return configs.Find(x => x.ItemType == itemType);
        }
    }
}