using System;
using System.Collections.Generic;
using UnityEngine;

namespace LUP
{
    [Serializable]
    public class PCRItemStaticData : IItemStaticData, UnityEngine.ISerializationCallbackReceiver
    {
        [Column("ItemID", Required = true)]
        public int ItemID;

        [Column("ItemName", Required = true)]
        public string ItemName;

        [Column("ItemType", Required = true)]
        public string ItemType;

        [Column("IconPath")]
        public string IconPath = "";

        [Column("MaxStackSize")]
        public int MaxStackSize = 1;

        [Column("IsUsable")]
        public bool IsUsable = false;

        [Column("Description")]
        public string Description = "";

        [System.NonSerialized]
        private Dictionary<string, string> customFields = new Dictionary<string, string>();


        [SerializeField]
        private List<CustomField> serializedCustomFields = new List<CustomField>();

        public LUPItemData ToItemData()
        {
            var item = new LUPItemData();

            item.SetItemID(ItemID);
            item.SetItemName(ItemName);
            item.SetItemType(ParseItemType(ItemType));

            if (!string.IsNullOrEmpty(IconPath))
                item.SetIconPath(IconPath);

            if (MaxStackSize != 1)
                item.SetMaxStackSize(MaxStackSize);

            if (IsUsable)
                item.SetIsUsable(IsUsable);

            if (!string.IsNullOrEmpty(Description))
                item.SetDescription(Description);

            if (customFields != null && customFields.Count > 0)
            {
                item.SetCustomFields(customFields);
            }

            return item;
        }

        public void SetCustomField(string fieldName, string value)
        {
            if (customFields == null)
            {
                customFields = new Dictionary<string, string>();
            }
            customFields[fieldName] = value;
        }

        private Define.ItemType ParseItemType(string type)
        {
            if (Enum.TryParse<Define.ItemType>(type, true, out var result))
            {
                return result;
            }
            UnityEngine.Debug.LogWarning($"[LUPItemStaticData] Invalid ItemType: {type}, defaulting to None");
            return Define.ItemType.None;
        }


        public void OnBeforeSerialize()
        {

            serializedCustomFields.Clear();
            if (customFields != null)
            {
                foreach (var kvp in customFields)
                {
                    serializedCustomFields.Add(new CustomField { key = kvp.Key, value = kvp.Value });
                }
            }
        }

        public void OnAfterDeserialize()
        {

            customFields = new Dictionary<string, string>();
            if (serializedCustomFields != null)
            {
                foreach (var field in serializedCustomFields)
                {
                    customFields[field.key] = field.value;
                }
            }
        }
    }
}
