using UnityEngine;

namespace LUP.PCR
{
    public class GlobalBlackboard : MonoBehaviour
    {
        public static GlobalBlackboard Instance { get; private set; }
        public WorkerBlackboard BB { get; private set; } = new WorkerBlackboard();







        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
        }
    }
}
