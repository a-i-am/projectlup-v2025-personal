[System.Serializable]
public class PCRConstructionStaticData
{
    [Column("BuildingType")] public int buildingType;
    [Column("Level")] public int level;
    [Column("ConstructionTime")] public float constructionTime;
    [Column("CostCount")] public int costCount;
    [Column("ResourceType1")] public int resourceType1;
    [Column("Amount1")] public int amount1;
    [Column("ResourceType2")] public int resourceType2;
    [Column("Amount2")] public int amount2;


}
