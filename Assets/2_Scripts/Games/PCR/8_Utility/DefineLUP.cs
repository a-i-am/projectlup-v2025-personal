using UnityEngine;

namespace LUP.PCR
{


    public static class GridSize
    {
        public static int x = 21;
        public static int y = 20;

        public static float mapZPos = -2.5f;
        public static float tileSize = 5f;
    }

    public enum TileType
    {
        NONE,
        PATH,
        WALL,
        BUILDING,
        LADDER,
    }

    public enum BuildingType
    {
        NONE,
        CONTROLTOWER,
        POWERSTATION,
        RESTAURANT,
        LABORATORY,
        WATERTREATMENTPLANT,
        WORKSTATION,
        WHEATFARM,
        MUSHROOMFARM,
        STONEMINE,
        IRONMINE,
        COALMINE,
        MOLEFARM,
        DAIRYFARM,
        LADDER
    }

    public enum WallType
    {
        NONE,
        DUST,
        STONE
    }

    public enum ResourceType
    {
        None,
        Stone,
        Coal,
        Iron,
        Wheat,
        Mushroom,
        Meat,
        Food,
        Power,
        Diamond
    }

    public enum PlacementResultType
    {
        SUCCESS,
        NOTENOUGHSPACE,
        LACKOFRESOURCE
    }

    public enum BuildState
    {
        UNDERCONSTRUCTION,
        COMPLETED
    }

    public enum FoodType
    {
        None,
        Bread,
        GrilledMushroom,
        MeatSoup,
    }

    public enum TaskType
    {
        Idle,
        Dig,
        Construct,
        BuildingWheatFarm,
        BuildingMushroomFarm,
    }

    public enum UIScreen
    {
        Main,
        Inventory,
        SelectConstrcut,
        FarmTask,
        ConstructionDecision,
        DigWall,
    }

    public enum WorkerActionState
    {
        Idle = 0,


        Farming = 10,
        Hammering = 11,
        Researching = 12,


        Eating = 20,

    }

}
