using R3;
using System;
using UnityEngine;

namespace LUP.PCR
{

    public abstract class BuildingBase : StructureBase
    {
        public static event Action<ProductableBuilding, FarmUIBtnType> OnGlobalUIRequest;

        protected ProductionRuntimeData runtimeData;

        protected BuildingInfo buildingInfo;
        protected ConstructionInfo constructionInfo;

        public BuildingStaticData buildingStaticData;
        public PCRConstructionStaticData currentConstructionData;
        public BuildingEvents buildingEvents;

        public ReactiveProperty<string> buildingName = new ReactiveProperty<string>("");

        public PCRResourceCenter resourceCenter;
        public GameObject ConstructScreen;
        public ConstructionOverlayView constructionOverlay;
        public BuildingActionMenu actionMenu;



        protected IBuildState currBuildState;
        public abstract void Init(ProductionRuntimeData runtimeData);
        public abstract void CompleteContruction();

        public abstract void Upgrade();

        public abstract void DeliverToInventory();

        public void OpenBuildingUI()
        {
            OpenBuildingUI(FarmUIBtnType.Product);
            Debug.Log("OpenUI");
        }
        public void CloseBuildingUI()
        {
            Debug.Log("CloseUI");
        }

        public void ChangeState(IBuildState state)
        {
            currBuildState?.Exit();
            currBuildState = state;
            currBuildState.Enter(this);
        }

        public BuildingInfo GetBuildingInfo()
        {
            return buildingInfo;
        }

        public void SetBuildingInfo(BuildingInfo buildingInfo)
        {
            this.buildingInfo = buildingInfo;
        }

        public ConstructionInfo GetConstructionInfo()
        {
            return constructionInfo;
        }

        public void SetConstructionInfo(ConstructionInfo constructionInfo)
        {
            this.constructionInfo = constructionInfo;
        }


        public void SetEntrance(Vector2Int pivotPos)
        {
            entrancePos = pivotPos;
        }


        protected virtual void Awake()
        {
            if (buildingEvents == null)
            {
                buildingEvents = new BuildingEvents();
            }
        }


        protected virtual void Start()
        {
            buildingEvents.OnBuildingSelected += ToggleActionMenu;

            if (actionMenu != null)
            {
                actionMenu.OnSelectMenu += HandleMenuSelection;
            }
        }
        private void ToggleActionMenu()
        {
            if (actionMenu != null)
            {
                actionMenu.Toggle();
            }
        }

        private void HandleMenuSelection(FarmUIBtnType tabType)
        {
            if (actionMenu != null)
            {
                actionMenu.Hide();
            }
                OpenBuildingUI(tabType);
        }
        public void OpenBuildingUI(FarmUIBtnType initTab)
        {
            if (this is ProductableBuilding productable)
            {
                OnGlobalUIRequest?.Invoke(productable, initTab);
            }
        }
    }
}
