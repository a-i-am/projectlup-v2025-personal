using UnityEngine;

namespace LUP.PCR
{
    public class BuildingLadder : BuildingBase
    {
        protected IBuildState constructState;
        protected IBuildState completeState;

        protected override void Awake()
        {
            constructState = new UnderConstructionState();
            completeState = new CompletedState();
        }

        protected override void Start()
        {
        }

        void Update()
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
            buildingName.Value = "Ladder";
            placeName = buildingName.Value;

            ProductionStage stage = LUP.StageManager.Instance.GetCurrentStage() as ProductionStage;
            currentConstructionData = stage.GetCurrentConstructionData((int)BuildingType.LADDER, buildingInfo.level);

            if (buildingInfo.isConstructing)
            {
                ChangeState(constructState);
            }
            else
            {
                ChangeState(completeState);
            }
        }

        public override void CompleteContruction()
        {
            ChangeState(completeState);
        }

        public override void Upgrade()
        {
        }

        public override void DeliverToInventory()
        {

        }
    }
}
