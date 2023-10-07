using System.Linq;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    [CreateAssetMenu(fileName = "SkinsHolderConfig", menuName = "Configs/SkinsHolderConfig")]
    public class ItemsHolderConfig : ScriptableObject
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private Item[] items;

        public ItemType ItemType => itemType;
        public Item[] Items => items;

        public Item GetItem(string itemName)
        {
            return items.ToList().Find(x => x.Name == itemName);
        }
    }
}