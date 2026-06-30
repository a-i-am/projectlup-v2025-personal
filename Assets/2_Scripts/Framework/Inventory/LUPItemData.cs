using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LUP
{
    [System.Serializable]
    public class LUPItemData : IItemable
    {
        [Header("필수 필드")]
        [SerializeField] private int itemID;
        [SerializeField] private string itemName;
        [SerializeField] private Define.ItemType itemType;
        [SerializeField] private string iconPath = "";
        [SerializeField] private int maxStackSize = 1;
        [SerializeField] private bool isUsable = false;
        [SerializeField] private string description = "";

        [System.NonSerialized] private Sprite _iconCache;


        [System.NonSerialized] private Dictionary<string, string> customFields = new Dictionary<string, string>();


        [SerializeField] private List<CustomField> serializedCustomFields = new List<CustomField>();

        public int ItemID => itemID;
        public string ItemName => itemName;
        public Define.ItemType Type => itemType;
        public int MaxStackSize => maxStackSize;
        public bool IsUsable => isUsable;
        public string Description => description;

        public Sprite Icon
        {
            get
            {
                if (_iconCache == null && !string.IsNullOrEmpty(iconPath))
                {
                    _iconCache = Resources.Load<Sprite>(iconPath);
                }
                return _iconCache;
            }
        }

        public void OnUse()
        {
            if (!isUsable)
            {
                Debug.LogWarning($"{itemName}은(는) 사용할 수 없는 아이템입니다.");
                return;
            }

            Debug.Log($"{itemName} 사용! {description}");
        }


        public string IconPath => iconPath;


        public void SetItemID(int id) => itemID = id;
        public void SetItemName(string name) => itemName = name;
        public void SetItemType(Define.ItemType type) => itemType = type;
        public void SetIconPath(string path) => iconPath = path;
        public void SetMaxStackSize(int size) => maxStackSize = size;
        public void SetIsUsable(bool usable) => isUsable = usable;
        public void SetDescription(string desc) => description = desc;



        public int GetInt(string fieldName, int defaultValue = 0)
        {
            if (customFields.TryGetValue(fieldName, out string value))
            {
                return int.TryParse(value, out int result) ? result : defaultValue;
            }
            return defaultValue;
        }

        public float GetFloat(string fieldName, float defaultValue = 0f)
        {
            if (customFields.TryGetValue(fieldName, out string value))
            {
                return float.TryParse(value, out float result) ? result : defaultValue;
            }
            return defaultValue;
        }

        public string GetString(string fieldName, string defaultValue = "")
        {
            return customFields.TryGetValue(fieldName, out string value) ? value : defaultValue;
        }

        public bool GetBool(string fieldName, bool defaultValue = false)
        {
            if (customFields.TryGetValue(fieldName, out string value))
            {
                return bool.TryParse(value, out bool result) ? result : defaultValue;
            }
            return defaultValue;
        }

        public void SetCustomField(string fieldName, string value)
        {
            if (customFields == null)
            {
                customFields = new Dictionary<string, string>();
            }
            customFields[fieldName] = value;
        }

        public void SetCustomFields(Dictionary<string, string> fields)
        {
            customFields = new Dictionary<string, string>(fields);
            SyncToSerializedList();
        }

        public bool HasCustomField(string fieldName)
        {
            return customFields != null && customFields.ContainsKey(fieldName);
        }

        public IEnumerable<string> GetCustomFieldNames()
        {
            return customFields?.Keys ?? Enumerable.Empty<string>();
        }

        public void MergeWith(LUPItemData other)
        {
            if (other == null || other.customFields == null) return;

            if (customFields == null)
            {
                customFields = new Dictionary<string, string>();
            }

            foreach (var kvp in other.customFields)
            {

                if (!customFields.ContainsKey(kvp.Key) || string.IsNullOrEmpty(customFields[kvp.Key]))
                {
                    customFields[kvp.Key] = kvp.Value;
                }
            }
            SyncToSerializedList();
        }

        private void SyncToSerializedList()
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

        public void SyncFromSerializedList()
        {
            customFields = new Dictionary<string, string>();
            foreach (var field in serializedCustomFields)
            {
                customFields[field.key] = field.value;
            }
        }
    }

    [System.Serializable]
    public class CustomField
    {
        public string key;
        public string value;
    }
}
