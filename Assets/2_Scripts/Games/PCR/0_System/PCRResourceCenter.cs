using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LUP.PCR
{
    public sealed class PCRResourceCenter : MonoBehaviour
    {
        private Inventory inventory;

        private readonly Dictionary<ResourceType, ReactiveProperty<int>> resourceMap = new();

        public ReadOnlyReactiveProperty<int> Observe(ResourceType type)
        {
            Ensure(type);
            return resourceMap[type].ToReadOnlyReactiveProperty();
        }


        private void Ensure(ResourceType type)
        {
            if (resourceMap.ContainsKey(type)) return;
            resourceMap[type] = new ReactiveProperty<int>(0);
        }


        public void InitResource()
        {
            ProductionStage stage = StageManager.Instance.GetCurrentStage() as ProductionStage;
            inventory = stage.PCRInven;

            SyncAllFromInventory();
        }

        public void SyncAllFromInventory()
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                if (type == ResourceType.None) continue;
                Ensure(type);
                resourceMap[type].Value = GetResourceAmount(type);
            }
        }

        public int GetResourceAmount(ResourceType type)
        {
            int index = 9900 + (int)type;
            return inventory.GetItemCount(index);
        }

        public void AddResource(ResourceType type, int amount)
        {
            string itemName = type.ToString();
            IItemable item = ItemManager.Instance.GetItem(itemName);
            if (item != null)
            {
                inventory.AddItem(item, amount);
                Ensure(type);
                resourceMap[type].Value = GetResourceAmount(type);
                return;
            }
        }

        public bool TryUseResource(ResourceType type, int amount)
        {
            if (amount <= 0) return true;

            var item = ItemManager.Instance.GetItem(type.ToString());

            if (item == null) return false;

            if (GetResourceAmount(type) < amount) return false;

            inventory.UseItem(item.ItemID, amount);
            Ensure(type);
            resourceMap[type].Value = GetResourceAmount(type);
            return true;
        }
    }
}
