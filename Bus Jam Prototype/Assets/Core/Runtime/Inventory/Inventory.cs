using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Inventory
{
    public abstract class Inventory
    {
        protected static readonly string prefix = "_";
        private Dictionary<Type, Item> storeItemDict = new();
        private Dictionary<Type, Action<int, int>> _valueChangeListeners = new();

        public int Get<TInventoryItem>() where TInventoryItem : Item
        {
            Type type = typeof(TInventoryItem);
            return GetInternal(type);
        }

        private int GetInternal(Type type)
        {
            return GetItem(type).amount;
        }

        public void Add<TInventoryItem>(int diff) where TInventoryItem : Item
        {
            AddInternal(typeof(TInventoryItem), diff);
        }

        private protected virtual void AddInternal(Type type, int diff)
        {
            if (diff == 0) return;
            Item item = GetItem(type);
            item.amount = Mathf.Clamp(item.amount + diff, item.minAmount, item.maxAmount);
            PostAddToItem(item);
            if (_valueChangeListeners.TryGetValue(type, out Action<int, int> action)) action?.Invoke(diff, item.amount);
        }

        private protected abstract void PostAddToItem(Item item);
        
        private Item GetItem(Type type)
        {
            Item storage;
            if (storeItemDict.TryGetValue(type, out Item storageObj))
            {
                return storageObj;
            }
            else
            {
                storage = CreateItem(type);
                storeItemDict.Add(type, storage);
            }

            return storage;
        }

        private Item CreateItem(Type type)
        {
            Item item = Activator.CreateInstance(type) as Item;
            PostCreateItem(item);
            return item;
        }

        private protected abstract void PostCreateItem(Item item);
    }
}