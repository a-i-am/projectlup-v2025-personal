using UnityEngine;

namespace LUP.PCR
{
    public class BuildingRestaurant : BuildingBase
    {
        protected IBuildState completeState;
        protected IBuildState cookingState;

        protected RestaurantInfo restaurantInfo;

        protected override void Awake()
        {
            buildingEvents = new BuildingEvents();
            completeState = new CompletedState();
            cookingState = new CookingState();
        }

        protected override void Start()
        {


            buildingEvents.OnBuildingDeselected += CloseBuildingUI;
        }

        private void Update()
        {

            float deltaTime = Time.deltaTime;
            currBuildState?.Tick(deltaTime);
        }

        public override void Init(ProductionRuntimeData runtimeData)
        {
            this.runtimeData = runtimeData;

            buildingName.Value = "Restaurant";
            placeName = buildingName.Value;

            if (ConstructScreen)
            {
                ConstructScreen.SetActive(false);
            }

            ProductionStage stage = LUP.StageManager.Instance.GetCurrentStage() as ProductionStage;
            restaurantInfo = stage.productionRuntimeData.RestaurantInfo;

            if (restaurantInfo == null)
            {
                restaurantInfo = new RestaurantInfo();
                stage.productionRuntimeData.RestaurantInfo = restaurantInfo;
            }

            if (restaurantInfo.isCooking)
            {
                ChangeState(cookingState);
            }
            else
            {
                ChangeState(completeState);
            }
        }

        public override void CompleteContruction() { }
        public override void Upgrade() { }

        public void SetCookingFood(FoodType food, int amount)
        {
            restaurantInfo.currentFood = food;
            restaurantInfo.totalCount = amount;
        }

        public void StartCooking()
        {
            CompletedState comState = currBuildState as CompletedState;
            if (comState != null)
            {

                ChangeState(cookingState);
            }

            CookingState cookState = currBuildState as CookingState;
            if (cookState != null)
            {
                cookState.Start();
            }
            else
            {
                Debug.Log("State is NOT CookingState");
            }
        }

        public void StopCooking()
        {
            CookingState state = currBuildState as CookingState;
            if (state != null)
            {
                state.Stop();
                ChangeState(completeState);
            }
            else
            {
                Debug.Log("State is NOT CookingState");
            }
        }

        public void CompleteCooking()
        {
            Debug.Log("CompleteCooking");

            DeliverToInventory();
            restaurantInfo.totalCount--;

            if (restaurantInfo.totalCount <= 0)
            {

                StopCooking();
            }
            else
            {
                StartCooking();
            }
        }

        public override void DeliverToInventory()
        {
            switch (restaurantInfo.currentFood)
            {
                case FoodType.Bread:
                    resourceCenter.TryUseResource(ResourceType.Wheat, 10);
                    resourceCenter.AddResource(ResourceType.Food, 50);
                    break;
                case FoodType.GrilledMushroom:
                    resourceCenter.TryUseResource(ResourceType.Mushroom, 10);
                    resourceCenter.AddResource(ResourceType.Food, 30);
                    break;
                case FoodType.MeatSoup:
                    resourceCenter.TryUseResource(ResourceType.Meat, 10);
                    resourceCenter.AddResource(ResourceType.Food, 70);
                    break;
                default:
                    resourceCenter.AddResource(ResourceType.Food, 0);
                    break;
            }
        }

        public RestaurantInfo GetRestaurantInfo()
        {
            return restaurantInfo;
        }
    }

}

