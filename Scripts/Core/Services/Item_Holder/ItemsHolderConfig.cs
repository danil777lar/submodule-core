using System;
using System.Linq;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    [Serializable]
    public abstract class ItemsHolderConfig : ScriptableObject
    {
        [field: SerializeField] public ItemType ItemType { get; protected set; }
        public abstract Item[] Items { get; }

        public Item GetItem(string itemName)
        {
            if (string.IsNullOrEmpty(itemName) || string.IsNullOrWhiteSpace(itemName))
            {
                return null;
            }

            return Items.ToList().Find(x => x.Name == itemName);
        }
    }
}