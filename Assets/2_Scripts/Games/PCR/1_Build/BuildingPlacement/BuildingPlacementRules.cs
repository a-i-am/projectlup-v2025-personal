using UnityEngine;

namespace LUP.PCR
{
    public class BuildingPlacementRules : MonoBehaviour
    {
        PlacementRules placementRules;

        private void Awake()
        {
            placementRules = gameObject.AddComponent<PlacementRules>();
        }

        public void Init(TileMap tileMap)
        {
            placementRules.Init(tileMap);
        }

        public PlacementResultType CanPlace(BuildingType type, Tile pivotTile)
        {
            placementRules.TransitionRules(type);

            if (placementRules.CheckSpaceAvailable(pivotTile) == false)
            {
                return PlacementResultType.NOTENOUGHSPACE;
            }

            if (placementRules.CheckResourceAvailable() == false)
            {
                return PlacementResultType.LACKOFRESOURCE;
            }

            return PlacementResultType.SUCCESS;
        }
    }
}
