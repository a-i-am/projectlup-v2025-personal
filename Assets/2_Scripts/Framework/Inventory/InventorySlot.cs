using UnityEngine;

namespace LUP
{
    public interface IItemable
    {
        int ItemID { get; }
        string ItemName { get; }
        LUP.Define.ItemType Type { get; }
        int MaxStackSize { get; }
        Sprite Icon { get; }
        string Description { get; }

        bool IsUsable { get; }

        void OnUse();

        int GetInt(string fieldName, int defaultValue = 0);
        float GetFloat(string fieldName, float defaultValue = 0f);
        string GetString(string fieldName, string defaultValue = "");
        bool GetBool(string fieldName, bool defaultValue = false);
        bool HasCustomField(string fieldName);
    }

    public class InventorySlot
    {
        public IItemable Item { get; private set; }
        public int Quantity { get; private set; }

        public bool IsEmpty => Item == null;
        public bool IsFull => Quantity >= Item.MaxStackSize;
        public bool CanStack => !IsFull;

        public InventorySlot(IItemable item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }

        public bool TryAddQuantity(int amount)
        {
            if (Quantity + amount > Item.MaxStackSize)
                return false;

            Quantity += amount;
            return true;
        }

        public bool TryRemoveQuantity(int amount)
        {
            if (Quantity < amount)
                return false;

            Quantity -= amount;
            return true;
        }

        public void Clear()
        {
            Item = null;
            Quantity = 0;
        }
    }
}

