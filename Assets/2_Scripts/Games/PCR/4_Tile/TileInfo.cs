using UnityEngine;

namespace LUP.PCR
{
    [System.Serializable]
    public struct TileInfo
    {
        public Vector2Int pos;
        public TileType tileType;
        public BuildingType buildingType;
        public WallType wallType;
        public int id;


        public TileInfo(TileType tileType, BuildingType buildingType, WallType wallType, Vector2Int pos, int id)
        {
            this.tileType = tileType;
            this.buildingType = buildingType;
            this.wallType = wallType;
            this.id = id;
            this.pos = pos;
        }
    }

}

