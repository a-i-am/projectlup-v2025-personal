using UnityEngine;

namespace LUP.PCR
{
    [System.Serializable]
    public class ProductionInfo
    {
        public int buildingId;
        public float elapsedTime;
        public int currentStorage;

        public ProductionInfo(int buildingId, float elapsedTime, int currentStorage)
        {
            this.buildingId = buildingId;
            this.elapsedTime = elapsedTime;
            this.currentStorage = currentStorage;
        }
    }
}
