using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LUP.PCR
{
    public readonly struct WorkerBlackboardKey : IEquatable<WorkerBlackboardKey>
    {
        public readonly string Key;
        public readonly int hashKey;
        public WorkerBlackboardKey(string key)
        {
            this.Key = key;
            hashKey = ComputeHash(Key);
        }
        private static int ComputeHash(string str)
        {
            unchecked
            {
                int hash = (int)2166136261;
                foreach (char c in str)
                    hash = (hash * 16777619) ^ c;
                return hash;
            }
        }

        public bool Equals(WorkerBlackboardKey other) => hashKey == other.hashKey;
        public override bool Equals(object obj) => obj is WorkerBlackboardKey other && Equals(other);
        public override int GetHashCode() => hashKey;
        public override string ToString() => Key;
        public static bool operator ==(WorkerBlackboardKey lhs, WorkerBlackboardKey rhs) => lhs.Equals(rhs);
        public static bool operator !=(WorkerBlackboardKey lhs, WorkerBlackboardKey rhs) => !(lhs == rhs);
    }
}
