using System.Collections.Generic;
using UnityEngine;

namespace LUP
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        private Dictionary<string, Inventory> inventories = new Dictionary<string, Inventory>();

        public override void Awake()
        {
            base.Awake();

            BaseRuntimeData.SetCoroutineRunner(this);

            Debug.Log("[InventoryManager] 초기화 완료");
        }

        public void RegisterInventory(string inventoryKey, Inventory inventory)
        {
            if (string.IsNullOrEmpty(inventoryKey))
            {
                Debug.LogError("[InventoryManager] inventoryKey가 비어있습니다!");
                return;
            }

            if (inventory == null)
            {
                Debug.LogError($"[InventoryManager] '{inventoryKey}' 인벤토리가 null입니다!");
                return;
            }

            if (inventories.ContainsKey(inventoryKey))
            {
                Debug.LogWarning($"[InventoryManager] '{inventoryKey}' 인벤토리가 이미 등록되어 있습니다. 덮어씁니다.");
            }

            inventories[inventoryKey] = inventory;
            Debug.Log($"[InventoryManager] '{inventoryKey}' 인벤토리 등록 완료");
        }

        public Inventory GetInventory(string inventoryKey)
        {
            if (string.IsNullOrEmpty(inventoryKey))
            {
                Debug.LogError("[InventoryManager] inventoryKey가 비어있습니다!");
                return null;
            }

            if (inventories.TryGetValue(inventoryKey, out Inventory inventory))
            {
                return inventory;
            }

            Debug.LogWarning($"[InventoryManager] '{inventoryKey}' 인벤토리를 찾을 수 없습니다.");
            return null;
        }

        public Inventory LoadOrCreateInventory(string inventoryKey, string filename)
        {
            if (string.IsNullOrEmpty(inventoryKey))
            {
                Debug.LogError("[InventoryManager] inventoryKey가 비어있습니다!");
                return null;
            }

            if (string.IsNullOrEmpty(filename))
            {
                Debug.LogError($"[InventoryManager] '{inventoryKey}' filename이 비어있습니다!");
                return null;
            }

            Inventory inventory;

            if (JsonDataHelper.FileExists(filename))
            {
                inventory = JsonDataHelper.LoadData<Inventory>(filename);
                if (inventory != null)
                {
                    inventory.filename = filename;
                    inventory.InitializeFromJson();
                    Debug.Log($"[InventoryManager] '{inventoryKey}' 인벤토리 로드 완료 (파일: {filename})");
                }
                else
                {
                    Debug.LogWarning($"[InventoryManager] '{inventoryKey}' 로드 실패, 새로 생성");
                    inventory = new Inventory();
                    inventory.filename = filename;
                }
            }
            else
            {

                inventory = new Inventory();
                inventory.filename = filename;
                Debug.Log($"[InventoryManager] '{inventoryKey}' 새 인벤토리 생성 (파일: {filename})");
            }


            RegisterInventory(inventoryKey, inventory);
            return inventory;
        }

        public bool HasInventory(string inventoryKey)
        {
            if (string.IsNullOrEmpty(inventoryKey))
            {
                return false;
            }

            return inventories.ContainsKey(inventoryKey);
        }

        public void SaveAllInventories()
        {
            if (inventories.Count == 0)
            {
                Debug.Log("[InventoryManager] 저장할 인벤토리가 없습니다.");
                return;
            }

            int savedCount = 0;
            foreach (var kvp in inventories)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.SaveData();
                    savedCount++;
                }
            }

            Debug.Log($"[InventoryManager] 모든 인벤토리 저장 완료 ({savedCount}/{inventories.Count}개)");
        }

        public bool UnregisterInventory(string inventoryKey)
        {
            if (string.IsNullOrEmpty(inventoryKey))
            {
                Debug.LogError("[InventoryManager] inventoryKey가 비어있습니다!");
                return false;
            }

            if (inventories.Remove(inventoryKey))
            {
                Debug.Log($"[InventoryManager] '{inventoryKey}' 인벤토리 제거됨");
                return true;
            }

            Debug.LogWarning($"[InventoryManager] '{inventoryKey}' 인벤토리를 찾을 수 없어 제거 실패");
            return false;
        }

        public List<string> GetAllInventoryKeys()
        {
            return new List<string>(inventories.Keys);
        }

        public int GetInventoryCount()
        {
            return inventories.Count;
        }

        public void ClearAllInventories()
        {
            foreach (var kvp in inventories)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.Clear();
                }
            }

            Debug.Log($"[InventoryManager] 모든 인벤토리 클리어 완료 ({inventories.Count}개)");
        }
    }
}
