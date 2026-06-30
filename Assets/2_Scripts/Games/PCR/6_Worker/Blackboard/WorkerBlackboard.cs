using System.Collections.Generic;
using UnityEngine;

namespace LUP.PCR
{
    public class WorkerBlackboard : MonoBehaviour
    {
        private Dictionary<WorkerBlackboardKey, object> data = new ();
        private Dictionary<string, WorkerBlackboardKey> keyRegistry = new ();


        public WorkerBlackboardKey GetOrRegisterKey(string keyName)
        {
            if (keyRegistry.TryGetValue(keyName, out WorkerBlackboardKey existingKey))
            {
                return existingKey;
            }

            WorkerBlackboardKey newKey = new WorkerBlackboardKey(keyName);
            keyRegistry[keyName] = newKey;

            return newKey;
        }

        public void SetValue<T>(string keyName, T value)
        {
            WorkerBlackboardKey key = GetOrRegisterKey(keyName);
            SetValue(key, value);
        }

        public void SetValue<T>(WorkerBlackboardKey key, T value)
        {
            data[key] = value;
        }

        public T GetValue<T>(string keyName)
        {
            if (keyRegistry.TryGetValue(keyName, out var key)) { return GetValue<T>(key); }
            return default(T);
        }
        public T GetValue<T>(WorkerBlackboardKey key)
        {
            if (data.TryGetValue(key, out object val))
            {

                if (val is T castedVal) return castedVal;
            }
            return default(T);
        }

        public bool HasKey(string keyName)
        {
            return keyRegistry.TryGetValue(keyName, out var key) && data.ContainsKey(key);
        }

        public bool TryGetValue<T>(string keyName, out T value)
        {
            if (keyRegistry.TryGetValue(keyName, out var key))
            {
                value = GetValue<T>(key);
                return true;
            }

            value = default;
            return false;
        }

        public void Remove(string keyName)
        {
            if (keyRegistry.TryGetValue(keyName, out var key))
                data.Remove(key);
        }
    }
}
