using UnityEngine;

namespace LUP.PCR
{
    public class PlacementRules : MonoBehaviour
    {
        protected TileMap tileMap;
        protected Vector2Int placementSize;

        public void Init(TileMap tileMap)
        {
            this.tileMap = tileMap;
        }

        public bool CheckSpaceAvailable(Tile pivotTile)
        {
            if (!tileMap)
            {
                Debug.Log("TileMap is empty!");
                return false;
            }

            int startGridX = pivotTile.tileInfo.pos.x;
            int startGridY = pivotTile.tileInfo.pos.y;

            if (startGridX + placementSize.x - 1 >= GridSize.x ||
                startGridY + placementSize.y + 1 < 0)
            {
                return false;
            }

            for (int i = 0; i < placementSize.x; i++)
            {
                for (int j = 0; j < placementSize.y; j++)
                {
                    int nextGridX = startGridX + i;
                    int nextGridY = startGridY - j;


                    if (tileMap.tiles[nextGridX, nextGridY].tileInfo.tileType != TileType.NONE &&
                        tileMap.tiles[nextGridX, nextGridY].tileInfo.tileType != TileType.PATH)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CheckResourceAvailable()
        {

            return true;
        }

        public void TransitionRules(BuildingType type)
        {
            placementSize = BuildingSizeTable.Get(type);
        }
    }

}
