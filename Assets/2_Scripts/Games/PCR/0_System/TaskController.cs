using UnityEngine;
using System;
using R3;

namespace LUP.PCR
{
    public class TaskController : MonoBehaviour
    {
        private ITaskState digWallState;
        private ITaskState buildingState;
        private ITaskState idleState;

        public DigWallPreview digWallPreview;
        public BuildPreview buildPreview;

        public BuildingType currSelectedBuildingType;
        public ITaskState currentState { get; set; }

        public TileMap tileMap;
        public Tile lastClickTile;

        public BuildingSystem buildingSystem;

        private Subject<TaskType> onTaskChanged = new();
        public Observable<TaskType> OnTaskChanged => onTaskChanged;

        private Subject<ProductableBuilding> onClickProductableBuilding = new();
        public Observable<ProductableBuilding> OnClickProductableBuilding => onClickProductableBuilding;

        private readonly CompositeDisposable cd = new();

        private void Awake()
        {
            idleState = new IdleState(this);
            digWallState = new DigWallState(this);
            buildingState = new BuildingState(this);
        }

        private void Start()
        {
            currentState = null;
            lastClickTile = null;
            currSelectedBuildingType = BuildingType.NONE;
        }

        private void Update()
        {
            if (currentState != null)
            {
                currentState.InputHandle();
            }
        }

        private void OnDestroy()
        {
            cd.Dispose();
        }

        public void InitTaskController(DigWallPreview digWallPreview, BuildPreview buildPreview,
            TileMap tileMap, BuildingSystem buildingSystem)
        {
            this.digWallPreview = digWallPreview;
            this.buildPreview = buildPreview;
            this.tileMap = tileMap;
            this.buildingSystem = buildingSystem;

            IdleTask();

            Debug.Log("TaskController Init");
        }

        public void Trasition(ITaskState state)
        {
            if (currentState != null)
            {
                currentState.Close();
            }
            currentState = state;
            currentState.Open();
        }

        private void NotifyTaskChanged(TaskType type)
        {
            onTaskChanged.OnNext(type);
        }

        public void NotifyBuildingClicked(BuildingBase building)
        {
            ProductableBuilding productableBuilding = building as ProductableBuilding;
            if (productableBuilding)
            {
                onClickProductableBuilding.OnNext(productableBuilding);
            }
        }

        public void DigWallTask()
        {
            NotifyTaskChanged(TaskType.Dig);
            Trasition(digWallState);
        }

        public void BuildingTask()
        {
            NotifyTaskChanged(TaskType.Construct);
            Trasition(buildingState);
        }

        public void IdleTask()
        {
            NotifyTaskChanged(TaskType.Idle);
            Trasition(idleState);
        }

        public void SetIdleActive(bool isActive)
        {
            IdleState idle = idleState as IdleState;
            idle.SetIsActiveUI(isActive);
        }

        public void SetCurrSelectedBuildingType(BuildingType buildingType)
        {
            currSelectedBuildingType = buildingType;
        }


        public void CreateBuilding()
        {
            buildingSystem.CreateBuilding(currSelectedBuildingType, lastClickTile);
        }






        public void ReturnToIdleState()
        {
            IdleTask();

        }

        public void UpdateLastClickTile(Tile tile)
        {
            lastClickTile = tile;
            Debug.Log("Current Tile Pos: (" + lastClickTile.tileInfo.pos.x + ", " + lastClickTile.tileInfo.pos.y + ")");
        }
    }

}
