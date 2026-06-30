using UnityEngine;

namespace LUP.PCR
{

    public class BuildingStoneMine : ProductableBuilding
    {

        protected override void Awake()
        {
            buildingEvents = new BuildingEvents();
            constructState = new UnderConstructionState();
            productableState = new ProductableState();
        }

        protected override void Start()
        {
            base.Start();


            buildingEvents.OnBuildingDeselected += CloseBuildingUI;
        }
        private void Update()
        {
            if (!hasWork)
            {
                return;
            }


            float deltaTime = Time.deltaTime;
            currBuildState?.Tick(deltaTime);
        }

        public override void Init(ProductionRuntimeData runtimeData)
        {
            this.runtimeData = runtimeData;


            productionInfo = runtimeData.GetProductionInfo(buildingInfo.buildingId);
            if (productionInfo == null)
            {
                ProductionInfo newProductionInfo = new ProductionInfo(buildingInfo.buildingId, 0f, 0);
                runtimeData.AddToList(runtimeData.ProductionInfoList, newProductionInfo);
                productionInfo = newProductionInfo;
            }


            constructionInfo = runtimeData.GetConstructionInfo(buildingInfo.buildingId);
            if (constructionInfo == null)
            {
                ConstructionInfo newConstructionInfo = new ConstructionInfo(buildingInfo.buildingId, 0f);
                runtimeData.AddToList(runtimeData.ConstructionInfoList, newConstructionInfo);
                constructionInfo = newConstructionInfo;
            }

            if (ConstructScreen)
            {
                ConstructScreen.SetActive(false);
            }


            hasWork = true;
            buildingName.Value = "StoneMine";
            placeName = buildingName.Value;


            ProductionStage stage = LUP.StageManager.Instance.GetCurrentStage() as ProductionStage;
            currentConstructionData = stage.GetCurrentConstructionData((int)BuildingType.STONEMINE, buildingInfo.level);
            currentProductionData = stage.GetCurrentProductionData((int)BuildingType.STONEMINE, buildingInfo.level);
            maxStorage.Value = currentProductionData.StorageCapacity;

            if (buildingInfo.isConstructing)
            {
                ChangeState(constructState);
            }
            else
            {
                ChangeState(productableState);
            }
        }


        public override void CompleteContruction()
        {

            buildingInfo.level++;
            ProductionStage stage = LUP.StageManager.Instance.GetCurrentStage() as ProductionStage;
            currentConstructionData = stage.GetCurrentConstructionData((int)BuildingType.STONEMINE, buildingInfo.level);
            currentProductionData = stage.GetCurrentProductionData((int)BuildingType.STONEMINE, buildingInfo.level);
            maxStorage.Value = currentProductionData.StorageCapacity;

            ChangeState(productableState);
        }
        public override void Upgrade()
        {
            ChangeState(productableState);
        }

        public override void SetupProductionData()
        {

        }

        public override void StartProduction()
        {
            ProductableState state = currBuildState as ProductableState;
            if (state != null)
            {
                state.Start();
            }
            else
            {
                Debug.Log("State is NOT Productable State");
            }
        }
        public override void StopProduction()
        {
            ProductableState state = currBuildState as ProductableState;
            if (state != null)
            {
                state.Stop();
            }
            else
            {
                Debug.Log("State is NOT Productable State");
            }
        }

        public override void CompleteProduction()
        {
            Debug.Log("CompleteProduction");
            productionInfo.currentStorage = productionInfo.currentStorage + 1 > maxStorage.Value ? maxStorage.Value : productionInfo.currentStorage + 1;

            if (productionInfo.currentStorage == maxStorage.Value)
            {
                DeliverToInventory();
                StartProduction();

            }
            else
            {
                StartProduction();
            }
        }

        public override void DeliverToInventory()
        {
            resourceCenter.AddResource(ResourceType.Stone, productionInfo.currentStorage);
            productionInfo.currentStorage = 0;
        }
    }

}
