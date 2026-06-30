using UnityEngine;

namespace LUP.PCR
{
    [System.Serializable]
    public class BuildingEvents
    {
        public System.Action OnBuildingSelected;
        public System.Action OnBuildingDeselected;
        public System.Action OnCompletedConstruction;
    }
}
