
namespace LUP.PCR
{
    public struct FarmUIData
    {
        public int level;
        public string buildingName;
        public int productionTime;
        public int curStorage;
        public int maxStorage;
        public int power;
        public bool isWorkRequested;
        public bool isConstructing;


        public int currentLevel;
        public bool isMaxLevel;


        public string effectName;
        public int currentStatValue;
        public int nextStatAddedValue;


        public int costType1;
        public int costAmount1;
        public int costType2;
        public int costAmount2;


        public void SetData(int level, string buildingName, int productionTime, int curStorage, int maxStorage, int power
            , bool isWorkRequested, bool isConstructing)
        {
            this.level = level;
            this.buildingName = buildingName;
            this.curStorage = curStorage;
            this.maxStorage = maxStorage;
            this.power = power;
            this.isWorkRequested = isWorkRequested;
            this.isConstructing = isConstructing;
        }
    }
}

