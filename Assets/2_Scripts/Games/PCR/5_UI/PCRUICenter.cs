using System;
using UnityEngine;
using R3;

namespace LUP.PCR
{
    public class PCRUICenter : MonoBehaviour
    {
        [Header("View")]
        [SerializeField] private MainUIView mainView;
        [SerializeField] private SelectConstructUIView selectConstructView;
        [SerializeField] private FarmTaskUIView farmTaskView;
        [SerializeField] private ConstructionDecisionView constructionDecisionView;
        [SerializeField] private InventoryUIView inventoryView;

        private MainUIViewModel mainVM;
        private InventoryUIViewModel invenVM;
        private SelectConstructViewModel selectConstructVM;
        private ConstructionDecisionViewModel constructionDecisionVM;
        private FarmTaskUIViewModel farmTaskUIVM;

        private readonly ReactiveProperty<UIScreen> screen = new(UIScreen.Main);
        private readonly CompositeDisposable cd = new();

        private BuildingBase currentBuilding;

        public void InitUI(TaskController controller, PCRResourceCenter resourceCenter)
        {
            mainVM = new MainUIViewModel(resourceCenter);
            invenVM = new InventoryUIViewModel(resourceCenter);
            selectConstructVM = new SelectConstructViewModel();
            constructionDecisionVM = new ConstructionDecisionViewModel();
            farmTaskUIVM = new FarmTaskUIViewModel();

            mainView.Bind(mainVM);
            inventoryView.Bind(invenVM);
            selectConstructView.Bind(selectConstructVM);
            constructionDecisionView.Bind(constructionDecisionVM);
            farmTaskView.Bind(farmTaskUIVM);

            // 구독
            mainVM.ClickConstruct.Subscribe(_ =>
            {
                controller.SetIdleActive(true);
                screen.Value = UIScreen.SelectConstrcut;
            }).AddTo(cd);
            mainVM.ClickDig.Subscribe(_ => 
            {
                controller.DigWallTask();
                screen.Value = UIScreen.DigWall;
            }).AddTo(cd);
            mainVM.ClickInventory.Subscribe(_ =>
            {
                screen.Value = UIScreen.Inventory;
            }).AddTo(cd);

            invenVM.ClickBack.Subscribe(_ =>
            {
                screen.Value = UIScreen.Main;
            }).AddTo(cd);

            selectConstructVM.ClickBack.Subscribe(_ => screen.Value = UIScreen.Main).AddTo(cd);
            selectConstructVM.OnBuildingSelected.Subscribe(buildingType =>
            {
                controller.SetCurrSelectedBuildingType(buildingType);
                controller.BuildingTask();
                
                screen.Value = UIScreen.ConstructionDecision;
            }).AddTo(cd);

            constructionDecisionVM.OnClickAccept.Subscribe(_ =>
            {
                controller.IdleTask();
                controller.CreateBuilding();
            }).AddTo(cd);
            constructionDecisionVM.OnClickReject.Subscribe(_ =>
            {
                controller.IdleTask();
            }).AddTo(cd);

            farmTaskUIVM.OnClickBack.Subscribe(_ =>
            {
                screen.Value = UIScreen.Main;
            }).AddTo(cd);
            farmTaskUIVM.OnClickUpgrade.Subscribe(_ =>
            {
                currentBuilding?.Upgrade();
            }).AddTo(cd);
            farmTaskUIVM.OnClickWorkRequest.Subscribe(_ =>
            {
                currentBuilding?.ToggleWorkRequest();
            }).AddTo(cd);

            screen.DistinctUntilChanged().Subscribe(TransitionScreen).AddTo(cd);
            controller.OnTaskChanged.Subscribe(HandleTaskChanged).AddTo(cd);
            controller.OnClickProductableBuilding.Subscribe(HandleProductableBuildingTask).AddTo(cd);

            // 초기 화면
            screen.Value = UIScreen.Main;

            Debug.Log("UICenter Init");
        }

        public void TransitionScreen(UIScreen scr)
        {
            // 초기화하는건데 애니메이션 만들 때는 없어야 할지도

            switch (scr)
            {
                case UIScreen.Main:
                    mainView.Show();
                    inventoryView.Hide();
                    selectConstructView.Hide();
                    farmTaskView.Hide();
                    constructionDecisionView.Hide();
                    break;
                case UIScreen.Inventory:
                    inventoryView.Show();
                    mainView.Hide();
                    break;
                case UIScreen.SelectConstrcut:
                    selectConstructView.Show();
                    mainView.Hide();
                    break;
                case UIScreen.FarmTask:
                    farmTaskView.Show();
                    mainView.Hide();
                    break;
                case UIScreen.ConstructionDecision:
                    constructionDecisionView.Show();
                    selectConstructView.Hide();
                    break;
                case UIScreen.DigWall:
                    mainView.Hide();
                    break;
            }
        }

        public void HandleTaskChanged(TaskType type)
        {
            switch (type)
            {
                case TaskType.Idle:
                    TransitionScreen(UIScreen.Main);
                    break;
                case TaskType.Dig:
                    TransitionScreen(UIScreen.DigWall);
                    break;
                case TaskType.Construct:
                    TransitionScreen(UIScreen.SelectConstrcut);
                    break;

            }
        }

        private void OnDestroy()
        {
            cd.Dispose();
        }

        public void HandleProductableBuildingTask(ProductableBuilding building)
        {
            currentBuilding = building;

            farmTaskUIVM.Observe(building);
            //TransitionScreen(UIScreen.FarmTask);
            screen.Value = UIScreen.FarmTask;
        }

        //public void OpenRestaurantTask(BuildingRestaurant building)
        //{
        //    currBuilding = building;

        //    // 추가 구현.            
        //}

    }

}
