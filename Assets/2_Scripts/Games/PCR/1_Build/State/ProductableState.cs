using UnityEngine;

namespace LUP.PCR
{
    public class ProductableState : IBuildState
    {
        public float totalTime;
        public float progressRatio;
        public bool isCompledted;
        public bool isStarted;

        private ProductableBuilding productableBuilding;
        private ProductionInfo currentProductionInfo;


        public void Enter(BuildingBase building)
        {
            Debug.Log("ProductableState Enter");

            productableBuilding = building as ProductableBuilding;
            currentProductionInfo = productableBuilding.GetProductionInfo();
            building.GetBuildingInfo().isConstructing = false;

            if (productableBuilding)
            {
                Start();
            }
        }

        public void Exit()
        {
            Debug.Log("ProductableState Exit");
            Reset();
        }

        public void Tick(float deltaTime)
        {
            if (!IsStarted())
            {
                return;
            }
            if (IsCompleted())
            {
                Debug.Log("ISComplete");
                return;
            }

            currentProductionInfo.elapsedTime += deltaTime;
            progressRatio = Mathf.Clamp01(currentProductionInfo.elapsedTime / totalTime);

            if (progressRatio >= 1f)
            {
                isCompledted = true;

                Complete();
            }
        }

        public void Complete()
        {
            productableBuilding.CompleteProduction();
        }

        public bool IsCompleted()
        {
            return isCompledted;
        }

        public bool IsStarted()
        {
            return isStarted;
        }

        public void Reset()
        {
            currentProductionInfo.elapsedTime = 0f;
            totalTime = 3600f / productableBuilding.currentProductionData.productionPerHour;
            progressRatio = 0f;
            isCompledted = false;
            isStarted = false;
        }

        public void Start()
        {
            Reset();
            isStarted = true;
            isCompledted = false;
        }

        public void Stop()
        {
            Reset();
        }
    }

}
