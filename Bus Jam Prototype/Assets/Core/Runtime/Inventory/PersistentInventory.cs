using UnityEngine;

namespace Core.Inventory
{
    public abstract class PersistentInventory : Inventory
    {
        protected abstract string Key { get; }

        private protected override void PostAddToItem(Item item)
        {
            PlayerPrefs.SetString(item.finalKey, item.amount.ToString());
        }

        private protected override void PostCreateItem(Item item)
        {
            item.finalKey = string.Join(prefix, Key, item.key);
            item.amount = int.Parse(PlayerPrefs.GetString(item.finalKey, 
                item.minAmount.ToString()));
        }
    }
}