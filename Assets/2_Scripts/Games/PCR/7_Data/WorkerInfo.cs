using UnityEngine;

namespace LUP.PCR
{
    [System.Serializable]
    public class WorkerInfo
    {
        public float hunger;
        bool hasTask;
        BuildingInfo currentTaskBuildingInfo;

        public int id;
        public string name;
        public StructureBase initPlace;

    }
}
