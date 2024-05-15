using System;
using System.Collections.Generic;
using ProjectConstants;

namespace Larje.Core.Services
{
    public interface IItemHolderService
    {
        public event Action EventCurrentItemChanged;
        public event Action EventNewItemUnlocked;

        public void UnlockItem(ItemType itemType, string itemName);

        public void SetCurrentItem(ItemType itemType, string itemName);

        public bool IsItemUnlocked(ItemType itemType, string itemName);

        public bool TryGetCurrentItem(out Item currentItem, ItemType itemType);

        public List<Item> GetAllItems(ItemType itemType);
    }
}