using System;
using System.Collections.Generic;
using ProjectConstants;

namespace Larje.Core.Services
{
    public partial class GameData
    {
        public List<ItemsData> ItemsData = new List<ItemsData>();
    }

    [Serializable]
    public class ItemsData
    {
        public ItemType ItemType;
        public string CurrentItem;
        public List<string> UnlockedItems = new List<string>();
    }
}