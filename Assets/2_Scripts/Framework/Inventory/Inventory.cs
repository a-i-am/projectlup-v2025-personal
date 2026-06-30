using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LUP
{
    [Serializable]
    public class Inventory : BaseRuntimeData, UnityEngine.ISerializationCallbackReceiver
    {

        [SerializeField]
        private List<InventorySlotData> _serializedSlots = new List<InventorySlotData>();

        [System.NonSerialized]
        private Dictionary<string, InventorySlot> slots;

        public event Action<IItemable, int> OnItemAdded;
        public event Action<IItemable, int> OnItemRemoved;
        public event Action<IItemable> OnItemUsed;

        public Inventory()
        {
            if (slots == null)
                slots = new Dictionary<string, InventorySlot>();
        }

        public bool AddItem(IItemable item, int quantity = 1)
        {
            if (item == null || quantity <= 0)
            {
                Debug.LogWarning("Invalid item or quantity");
                return false;
            }

            if (slots == null)
                slots = new Dictionary<string, InventorySlot>();

            InventorySlot stackableSlot = null;
            foreach (var slot in slots.Values)
            {
                if (slot.Item.ItemID == item.ItemID && slot.CanStack)
                {
                    stackableSlot = slot;
                    break;
                }
            }

            if (stackableSlot != null)
            {
                if (stackableSlot.TryAddQuantity(quantity))
                {
                    OnItemAdded?.Invoke(item, quantity);
                    Debug.Log($"[Inventory] AddItem: {item.ItemName} x{quantity} (스택 추가)");
                    NotifyValueChanged();
                    return true;
                }

                int remaining = quantity - (item.MaxStackSize - stackableSlot.Quantity);
                stackableSlot.TryAddQuantity(item.MaxStackSize - stackableSlot.Quantity);

                OnItemAdded?.Invoke(item, item.MaxStackSize - stackableSlot.Quantity);
                Debug.Log($"[Inventory] AddItem: {item.ItemName} 일부 추가, 남은 수량: {remaining}");
                NotifyValueChanged();

                return AddItem(item, remaining);
            }

            string slotKey = GenerateSlotKey(item.ItemID);
            slots[slotKey] = new InventorySlot(item, quantity);

            OnItemAdded?.Invoke(item, quantity);
            Debug.Log($"[Inventory] AddItem: {item.ItemName} x{quantity} (새 슬롯: {slotKey})");
            NotifyValueChanged();
            return true;
        }

        public bool UseItem(int itemID, int quantity = 1)
        {
            if (slots == null || !slots.TryGetValue(itemID.ToString(), out var slot))
            {
                Debug.LogWarning($"Item {itemID} not found");
                return false;
            }

            if (!slot.Item.IsUsable)
            {
                Debug.LogWarning($"Item {slot.Item.ItemName} is not usable");
                return false;
            }

            if (slot.Quantity < quantity)
            {
                Debug.LogWarning($"Not enough {slot.Item.ItemName}. Required: {quantity}, Have: {slot.Quantity}");
                return false;
            }

            for (int i = 0; i < quantity; i++)
            {
                slot.Item.OnUse();
                OnItemUsed?.Invoke(slot.Item);
            }

            RemoveItem(itemID, quantity);
            return true;
        }

        public bool RemoveItem(int itemID, int quantity = 1)
        {
            if (slots == null)
                return false;

            string key = itemID.ToString();

            if (!slots.TryGetValue(key, out var slot))
            {
                for(int i = 1; i < 20; i++)
                {
                    string alternateKey = string.Concat(key, "_",i);
                    if (slots.TryGetValue(alternateKey, out var altslot))
                    {
                        key = alternateKey;
                        slot = altslot; break;
                    }
                    continue;
                }
            }


            if (slot.Quantity < quantity)
                return false;

            slot.TryRemoveQuantity(quantity);

            if (slot.Quantity <= 0)
            {
                slots.Remove(key);
            }

            OnItemRemoved?.Invoke(slot.Item, quantity);
            NotifyValueChanged();
            return true;
        }

        public int GetItemCount(int itemID)
        {
            if (slots == null)
                return 0;

            int total = 0;
            foreach (var slot in slots.Values)
            {
                if (slot.Item.ItemID == itemID)
                {
                    total += slot.Quantity;
                }
            }
            return total;
        }

        public bool HasItem(int itemID, int requiredQuantity = 1)
        {
            return GetItemCount(itemID) >= requiredQuantity;
        }

        public List<InventorySlot> GetAllItems()
        {
            if (slots == null)
                return new List<InventorySlot>();

            return slots.Values.ToList();
        }

        public void Clear()
        {
            if (slots == null)
                slots = new Dictionary<string, InventorySlot>();

            slots.Clear();
            NotifyValueChanged();
        }

        private string GenerateSlotKey(int itemID)
        {
            int counter = 0;
            string key = itemID.ToString();

            while (slots.ContainsKey(key))
            {
                counter++;
                key = $"{itemID}_{counter}";
            }

            return key;
        }

        public override void ResetData()
        {
            if (slots == null)
                slots = new Dictionary<string, InventorySlot>();

            slots.Clear();
            _serializedSlots.Clear();
        }

        public void OnBeforeSerialize()
        {

            SyncToSerializedList();
        }

        public void OnAfterDeserialize()
        {

        }

        private void SyncToSerializedList()
        {
            _serializedSlots.Clear();

            if (slots == null)
            {
                Debug.LogWarning("[Inventory] SyncToSerializedList: slots가 null입니다!");
                return;
            }

            foreach (var kvp in slots)
            {
                _serializedSlots.Add(new InventorySlotData
                {
                    slotKey = kvp.Key,
                    itemID = kvp.Value.Item.ItemID,
                    quantity = kvp.Value.Quantity
                });
            }


        }

        public void InitializeFromJson()
        {
            if (slots == null)
                slots = new Dictionary<string, InventorySlot>();

            slots.Clear();

            if (_serializedSlots == null || _serializedSlots.Count == 0)
            {
                Debug.Log("[Inventory] 저장된 슬롯이 없습니다.");
                return;
            }


            if (ItemManager.Instance == null)
            {
                Debug.LogError("[Inventory] ItemManager가 초기화되지 않았습니다!");
                return;
            }

            int loadedCount = 0;
            int failedCount = 0;

            foreach (var slotData in _serializedSlots)
            {
                if (slotData == null || slotData.itemID == 0)
                {
                    failedCount++;
                    continue;
                }

                IItemable item = ItemManager.Instance.GetItem(slotData.itemID);
                if (item != null)
                {
                    slots[slotData.slotKey] = new InventorySlot(item, slotData.quantity);
                    loadedCount++;
                }
                else
                {
                    Debug.LogWarning($"[Inventory] 아이템을 찾을 수 없습니다: {slotData.itemID}");
                    failedCount++;
                }
            }

            Debug.Log($"[Inventory] 로드 완료: {loadedCount}개 성공, {failedCount}개 실패");
        }
    }
}
