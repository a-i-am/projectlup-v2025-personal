using LUP.PCR;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProductionRuntimeData : BaseRuntimeData
{
    [SerializeField] private int playerId;
    [SerializeField] private bool hasSavedGame;
    [SerializeField] private int buildingId;

    [SerializeField] private List<LUP.PCR.BuildingInfo> buildingInfoList = new List<LUP.PCR.BuildingInfo>();
    [SerializeField] private List<LUP.PCR.ProductionInfo> productionInfoList = new List<LUP.PCR.ProductionInfo>();
    [SerializeField] private List<LUP.PCR.ConstructionInfo> constructionInfoList = new List<LUP.PCR.ConstructionInfo>();
    [SerializeField] private LUP.PCR.RestaurantInfo restaurantInfo;
    [SerializeField] private List<LUP.PCR.WallInfo> wallInfoList = new List<LUP.PCR.WallInfo>();

    [SerializeField] private List<int> reservedBuildingIdList = new List<int>();
    [SerializeField] private List<int> assignedBuildingIdList = new List<int>();
    [SerializeField] private List<LUP.PCR.WorkerInfo> workerInfoList = new List<LUP.PCR.WorkerInfo>();

    public int PlayerId
    {
        get => playerId;
        set => SetValue(ref playerId, value);
    }

    public bool HasSavedGame
    {
        get => hasSavedGame;
        set => SetValue(ref hasSavedGame, value);
    }

    public int BuildingId
    {
        get => buildingId;
        set => SetValue(ref buildingId, value);
    }

    public List<LUP.PCR.BuildingInfo> BuildingInfoList
    {
        get => buildingInfoList;
        set => SetValue(ref buildingInfoList, value);
    }

    public List<LUP.PCR.WallInfo> WallInfoList
    {
        get => wallInfoList;
        set => SetValue(ref wallInfoList, value);
    }

    public List<LUP.PCR.ProductionInfo> ProductionInfoList
    {
        get => productionInfoList;
        set => SetValue(ref productionInfoList, value);
    }

    public List<LUP.PCR.ConstructionInfo> ConstructionInfoList
    {
        get => constructionInfoList;
        set => SetValue(ref constructionInfoList, value);
    }

    public LUP.PCR.RestaurantInfo RestaurantInfo
    {
        get => restaurantInfo;
        set => SetValue(ref restaurantInfo, value);
    }

    public List<int> ReservedBuildingIdList
    {
        get => reservedBuildingIdList;
        set => SetValue(ref reservedBuildingIdList, value);
    }

    public List<int> AssignedBuildingIdList
    {
        get => assignedBuildingIdList;
        set => SetValue(ref assignedBuildingIdList, value);
    }

    public List<LUP.PCR.WorkerInfo> WorkerInfoList
    {
        get => workerInfoList;
        set => SetValue(ref workerInfoList, value);
    }

    public override void ResetData()
    {
        buildingInfoList.Clear();
        productionInfoList.Clear();
        constructionInfoList.Clear();

        wallInfoList.Clear();
    }

    public void SaveProductionDatas()
    {
        NotifyValueChanged();
    }

    public LUP.PCR.BuildingInfo GetBuildingInfo(int buildingId)
    {
        foreach (LUP.PCR.BuildingInfo info in buildingInfoList)
        {
            if (info.buildingId == buildingId)
            {
                return info;
            }
        }

        return null;
    }

    public LUP.PCR.ProductionInfo GetProductionInfo(int buildingId)
    {
        foreach (LUP.PCR.ProductionInfo info in productionInfoList)
        {
            if (info.buildingId == buildingId)
            {
                return info;
            }
        }
        return null;
    }

    public LUP.PCR.ConstructionInfo GetConstructionInfo(int buildingId)
    {
        foreach (LUP.PCR.ConstructionInfo info in constructionInfoList)
        {
            if (info.buildingId == buildingId)
            {
                return info;
            }
        }
        return null;
    }

    public List<LUP.PCR.WallInfo> GetWallInfoList()
    {
        return wallInfoList;
    }

    public int GenerateId()
    {
        return BuildingId++;
    }

}
