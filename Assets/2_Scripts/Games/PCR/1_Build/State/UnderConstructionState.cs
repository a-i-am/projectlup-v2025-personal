using UnityEngine;

namespace LUP.PCR
{
    public class UnderConstructionState : IBuildState
    {
        public float totalTime;
        public float progressRatio;
        public bool isCompledted;
        public bool isStarted;

        private BuildingBase building;
        private ConstructionInfo currentConstructionInfo;

        public void Enter(BuildingBase building)
        {
            Debug.Log("UnderContructionState Enter");

            this.building = building;


            if (building.ConstructScreen)
            {
                building.ConstructScreen.SetActive(true);
            }

            if (building.constructionOverlay != null)
            {
                building.constructionOverlay.Show();
            }

            currentConstructionInfo = building.GetConstructionInfo();
            building.GetBuildingInfo().isConstructing = true;

            Start();
        }
        public void Exit()
        {
            if (building.ConstructScreen)
            {
                building.ConstructScreen.SetActive(false);
            }

            if (building.constructionOverlay != null)
            {
                building.constructionOverlay.Hide();
            }

            Stop();

            Debug.Log("UnderContructionState Exit");
        }
        public void Tick(float deltaTime)
        {
            if (!isStarted)
            {
                return;
            }
            if (isCompledted)
            {
                return;
            }

            currentConstructionInfo.elapsedTime += deltaTime;
            progressRatio = Mathf.Clamp01(currentConstructionInfo.elapsedTime / totalTime);

            if (building.constructionOverlay != null)
            {
                float remainingTime = Mathf.Max(0, totalTime - currentConstructionInfo.elapsedTime);
                building.constructionOverlay.UpdateView(progressRatio, remainingTime);
            }

            if (progressRatio >= 1f)
            {
                isCompledted = true;
            }

            if (isCompledted)
            {
                building.CompleteContruction();
            }
        }

        public void Reset()
        {
            totalTime = building.currentConstructionData.constructionTime;
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
            currentConstructionInfo.elapsedTime = 0f;
        }
    }
}
