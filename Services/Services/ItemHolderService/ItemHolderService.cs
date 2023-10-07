using System;
using System.Collections.Generic;
using System.Linq;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    [BindService(typeof(IItemHolderService))]
    public class ItemHolderService : Service, IItemHolderService
    {
        [SerializeField] private List<ItemsHolderConfig> configs;

        [InjectService] private DataService _dataService;

        public event Action EventCurrentItemChanged;
        public event Action EventNewItemUnlocked;

        public override void Init()
        {
        }

        public void UnlockItem(ItemType itemType, string itemName)
        {
            GetSkinsData(itemType).UnlockedItems.Add(itemName);
            EventNewItemUnlocked?.Invoke();
        }

        public void SetCurrentItem(ItemType itemType, string itemName)
        {
            if (IsItemUnlocked(itemType, itemName) && GetCurrentItem(itemType).Name != itemName)
            {
                GetSkinsData(itemType).CurrentItem = itemName;
                EventCurrentItemChanged?.Invoke();
            }
        }

        public bool IsItemUnlocked(ItemType itemType, string itemName)
        {
            return GetSkinsData(itemType).UnlockedItems.Contains(itemName);
        }

        public Item GetCurrentItem(ItemType itemType)
        {
            return GetConfigByType(itemType).GetItem(GetSkinsData(itemType).CurrentItem);
        }

        public List<Item> GetAllItems(ItemType itemType)
        {
            return GetConfigByType(itemType).Items.ToList();
        }

        private ItemsData GetSkinsData(ItemType itemType)
        {
            ItemsData data = _dataService.Data.ItemsData
                .ToList().Find(x => x.ItemType == itemType);

            if (data == null)
            {
                string defaultSkin = GetConfigByType(itemType).Items[0].Name;

                data = new ItemsData();
                data.ItemType = itemType;
                data.CurrentItem = defaultSkin;
                data.UnlockedItems.Add(defaultSkin);
                _dataService.Data.ItemsData.Add(data);
                _dataService.Save();
            }

            return data;
        }

        private ItemsHolderConfig GetConfigByType(ItemType itemType)
        {
            return configs.Find(x => x.ItemType == itemType);
        }
    }
}