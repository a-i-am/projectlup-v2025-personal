using System.Collections.Generic;
using UnityEngine;

namespace LUP.PCR
{
    public class BuildingGenerator : MonoBehaviour
    {
        [SerializeField]
        private Transform buildingSpawnTransform;
        [SerializeField]
        private Transform wallSpawnTransform;

        [SerializeField]
        private GameObject wheatFarmPrefab;
        [SerializeField]
        private GameObject moleFarmPrefab;
        [SerializeField]
        private GameObject restaurantPrefab;
        [SerializeField]
        private GameObject powerStationPrefab;
        [SerializeField]
        private GameObject stoneMinePrefab;
        [SerializeField]
        private GameObject workStationPrefab;
        [SerializeField]
        private GameObject ladderPrefab;


        [SerializeField]
        private GameObject dustPrefab;

        public GameObject CreateWall(WallInfo wallInfo)
        {
            GameObject wallObject = null;

            switch ((WallType)wallInfo.wallType)
            {
                case WallType.DUST:
                    wallObject = Instantiate(dustPrefab, new Vector3(wallInfo.gridPos.x * 5 + 2.5f, -wallInfo.gridPos.y * 5 - 2.5f, -2.5f), Quaternion.identity, wallSpawnTransform);

                    break;
                case WallType.STONE:

                    break;
            }

            return wallObject;
        }

        public BuildingBase CreateBuilding(BuildingType type, Tile pivotTile)
        {
            GameObject buildingObject = null;
            Vector3 pos = pivotTile.gameObject.transform.position;

            switch (type)
            {
                case BuildingType.WHEATFARM:
                    buildingObject = Instantiate(wheatFarmPrefab, pos, Quaternion.identity, buildingSpawnTransform);

                    break;
                case BuildingType.MOLEFARM:
                    buildingObject = Instantiate(moleFarmPrefab, pos, Quaternion.identity, buildingSpawnTransform);

                    break;
                case BuildingType.RESTAURANT:
                    buildingObject = Instantiate(restaurantPrefab, pos, Quaternion.identity, buildingSpawnTransform);

                    break;
                case BuildingType.POWERSTATION:
                    buildingObject = Instantiate(powerStationPrefab, pos, Quaternion.identity, buildingSpawnTransform);

                    break;
                case BuildingType.STONEMINE:
                    buildingObject = Instantiate(stoneMinePrefab, pos, Quaternion.identity, buildingSpawnTransform);

                    break;
                case BuildingType.WORKSTATION:
                    buildingObject = Instantiate(workStationPrefab, pos, Quaternion.identity, buildingSpawnTransform);

                    break;
                case BuildingType.LADDER:
                    buildingObject = Instantiate(ladderPrefab, pos, Quaternion.identity, buildingSpawnTransform);

                    break;
            }

            if (buildingObject == null)
            {
                return null;
            }

            BuildingBase building = buildingObject.GetComponent<BuildingBase>();
            return building;
        }

    }


}
