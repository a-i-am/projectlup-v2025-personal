using UnityEngine;

namespace LUP.PCR
{
    public class BuildPreview : MonoBehaviour
    {
        [SerializeField]
        GameObject wheatFarmPreview;
        [SerializeField]
        GameObject powerStationPreview;
        [SerializeField]
        GameObject stoneMinePreview;
        [SerializeField]
        GameObject moleFarmPreview;
        [SerializeField]
        GameObject workStationPreview;
        [SerializeField]
        GameObject restaurantPreview;
        [SerializeField]
        GameObject ladderPreview;

        [SerializeField]
        Material canBuildMaterial;
        [SerializeField]
        Material canNotBuildMaterial;

        private TileMap tileMap;
        private BuildingPlacementRules buildingPlacementRules;

        GameObject currPreview;

        public bool canBuild;

        private void Awake()
        {
            buildingPlacementRules = gameObject.AddComponent<BuildingPlacementRules>();
        }

        private void Start()
        {
            wheatFarmPreview.SetActive(false);
            powerStationPreview.SetActive(false);
            stoneMinePreview.SetActive(false);
            canBuild = false;
        }

        public void Init(TileMap tileMap)
        {
            this.tileMap = tileMap;
            buildingPlacementRules.Init(tileMap);
            currPreview = null;

            Debug.Log("BuildPreview Init");
        }

        public void ResetPreview()
        {
            currPreview.SetActive(false);
        }

        public void UpdatePreview(BuildingType type, Tile tile)
        {
            if (tile == null)
            {
                return;
            }

            currPreview.SetActive(true);
            Vector3 newPos = new Vector3(tile.gameObject.transform.position.x, tile.gameObject.transform.position.y, tile.gameObject.transform.position.z);


            currPreview.transform.position = tile.gameObject.transform.position;

            if (CanPlace(type, tile))
            {
                currPreview.GetComponentInChildren<MeshRenderer>().material = canBuildMaterial;
                canBuild = true;
            }
            else
            {
                currPreview.GetComponentInChildren<MeshRenderer>().material = canNotBuildMaterial;
                canBuild = false;
            }
        }

        public bool CanPlace(BuildingType type, Tile pivotTile)
        {
            switch (buildingPlacementRules.CanPlace(type, pivotTile))
            {
                case PlacementResultType.SUCCESS:
                    return true;

                case PlacementResultType.NOTENOUGHSPACE:
                case PlacementResultType.LACKOFRESOURCE:
                    return false;

            }

            return false;
        }

        public void ChangePreview(BuildingType type)
        {
            if (currPreview)
            {
                currPreview.SetActive(false);
            }

            switch (type)
            {
                case BuildingType.WHEATFARM:
                    currPreview = wheatFarmPreview;
                    break;

                case BuildingType.MUSHROOMFARM:

                    break;

                case BuildingType.MOLEFARM:
                    currPreview = moleFarmPreview;
                    break;

                case BuildingType.POWERSTATION:
                    currPreview = powerStationPreview;
                    break;
                case BuildingType.STONEMINE:
                    currPreview = stoneMinePreview;
                    break;
                case BuildingType.WORKSTATION:
                    currPreview = workStationPreview;
                    break;
                case BuildingType.RESTAURANT:
                    currPreview = restaurantPreview;
                    break;
                case BuildingType.LADDER:
                    currPreview = ladderPreview;
                    break;
                default:

                    break;
            }

            currPreview.SetActive(false);
        }
    }

}
