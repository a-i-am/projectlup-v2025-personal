using UnityEngine;

namespace LUP.PCR
{
    public class CookingState : IBuildState
    {
        public float cookTime;
        public float progressRatio;
        public bool isCompledted;
        public bool isStarted;

        private BuildingRestaurant restaurant;
        private RestaurantInfo restaurantInfo;


        public void Enter(BuildingBase building)
        {
            Debug.Log("CookingState Enter");

            if (restaurant == null)
            {
                restaurant = building as BuildingRestaurant;
                restaurantInfo = restaurant.GetRestaurantInfo();
            }

            Start();
        }
        public void Exit()
        {
            Debug.Log("CookingState Exit");

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

            restaurantInfo.elapsedTime += deltaTime;
            progressRatio = Mathf.Clamp01(restaurantInfo.elapsedTime / cookTime);

            if (progressRatio >= 1f)
            {
                isCompledted = true;
                Complete();
            }


        }

        public void Complete()
        {
            restaurant.CompleteCooking();
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
            restaurantInfo.elapsedTime = 0f;

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


