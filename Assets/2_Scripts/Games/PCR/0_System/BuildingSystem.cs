using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace LUP.PCR
{
    public class BuildingSystem : MonoBehaviour
    {
        private BuildingGenerator buildingGenerator;
        private TileMap tileMap;
        private PCRResourceCenter resourceCenter;

        private List<WallInfo> curWallInfoList;
        private List<BuildingInfo> curBuildingInfoList;
        private List<ProductionInfo> curProductionInfoList;
        private List<ConstructionInfo> curConstructionInfoList;

        private Dictionary<int, BuildingBase> currBuildings;

        private BuildPreview buildPreview;
        private DigWallPreview digWallPreview;

        private ProductionRuntimeData pcrRuntimeData;

        public void InitBuildingSystem(BuildingGenerator buildingGenerator,
            BuildPreview buildPreview, DigWallPreview digWallPreview, TileMap tileMap, PCRResourceCenter resourceCenter)
        {
            this.buildingGenerator = buildingGenerator;
            this.buildPreview = buildPreview;
            this.digWallPreview = digWallPreview;
            this.tileMap = tileMap;
            this.resourceCenter = resourceCenter;

            currBuildings = new Dictionary<int, BuildingBase>();

            ProductionStage stage = StageManager.Instance.GetCurrentStage() as ProductionStage;

            pcrRuntimeData = stage.productionRuntimeData;

            curBuildingInfoList = pcrRuntimeData.BuildingInfoList;
            curWallInfoList = pcrRuntimeData.WallInfoList;
            curProductionInfoList = pcrRuntimeData.ProductionInfoList;
            curConstructionInfoList = pcrRuntimeData.ConstructionInfoList;

            foreach (WallInfo wallInfo in curWallInfoList)
            {
                GameObject wallObject = buildingGenerator.CreateWall(wallInfo);
                if (!wallObject)
                {
                    Debug.Log("wallObject is null");
                    continue;
                }

                WallBase wall = wallObject.GetComponent<WallBase>();
                if (!wall)
                {
                    Debug.Log("WallBase is Null");
                    continue;
                }

                wall.SetWallInfo(wallInfo);
                tileMap.UpdateTilebyWall((WallType)wallInfo.wallType, wallInfo.gridPos);
            }

            foreach(BuildingInfo buildingInfo in curBuildingInfoList)
            {
                CreateInitialBuilding(buildingInfo);
            }

            tileMap.UpdateVision();

            Debug.Log("BuildingSystem Init");
        }
        public void RemoveWall(WallBase wall)
        {
            pcrRuntimeData.RemoveFromList(curWallInfoList, wall.GetWallInfo());

            UpdateDigTile(wall);
            Destroy(wall.gameObject);
            tileMap.UpdateVision();
        }

        public void UpdateDigTile(WallBase wall)
        {
            Tile tile = tileMap.GetTile(wall.GetWallInfo().gridPos);

            if (tile)
            {
                if (tile.tileInfo.tileType == TileType.WALL)
                {
                    digWallPreview.RemoveCanDigTile(tile);
                    tile.HideCanDigWallMark();
                    digWallPreview.AddCanNotDigTile(tile);
                    tile.SetTileInfo(new TileInfo(TileType.PATH, BuildingType.NONE, WallType.NONE, tile.tileInfo.pos, tile.tileInfo.id));
                    tile.UpdateVisualState();
                }
            }
        }

        public void CreateInitialBuilding(BuildingInfo buildingInfo)
        {
            BuildingBase building = buildingGenerator.CreateBuilding((BuildingType)buildingInfo.buildingType, tileMap.GetTile(buildingInfo.gridPos));

            if (building != null)
            {
                building.resourceCenter = resourceCenter;

                if (!currBuildings.ContainsKey(buildingInfo.buildingId))
                {
                    currBuildings.Add(buildingInfo.buildingId, building);
                }

                Tile pivotTile = tileMap.GetTile(buildingInfo.gridPos);

                tileMap.UpdateTilebyBuilding((BuildingType)buildingInfo.buildingType, pivotTile);
                building.SetEntrance(pivotTile.tileInfo.pos);
                building.SetBuildingInfo(buildingInfo);

                building.Init(pcrRuntimeData);
            }
        }

        public void CreateBuilding(BuildingType type, Tile pivotTile)
        {
            if (buildPreview.canBuild == false)
            {
                Debug.Log("Can't build");
                return;
            }

            BuildingBase building = buildingGenerator.CreateBuilding(type, pivotTile);

            if (building != null)
            {
                // @TODO: Id 설정 만들어야 한다.

                building.resourceCenter = resourceCenter;

                ProductionStage stage = StageManager.Instance.GetCurrentStage() as ProductionStage;
                if (stage == null) return;
                ProductionRuntimeData runtimeData = stage.productionRuntimeData as ProductionRuntimeData;

                int id = runtimeData.GenerateId();

                if (!currBuildings.ContainsKey(id))
                {
                    currBuildings.Add(id, building);
                }

                tileMap.UpdateTilebyBuilding(type, pivotTile);
                building.SetEntrance(pivotTile.tileInfo.pos);

                BuildingInfo newBuildingInfo = new BuildingInfo(id, 0, pivotTile.tileInfo.pos, (int)type, true);
                building.SetBuildingInfo(newBuildingInfo);

                pcrRuntimeData.AddToList(curBuildingInfoList, newBuildingInfo);
            }
        }

        // @TODO : 건물 편집 과정에서, 배치된 건물 이동이나 삭제도 가능하게 하기

        public Dictionary<int, BuildingBase> GetCurrentBuildingDictionary()
        {
            return currBuildings;
        }
    }
}
