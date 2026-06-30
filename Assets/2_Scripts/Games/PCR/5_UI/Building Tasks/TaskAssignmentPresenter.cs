using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LUP.PCR
{
    public class TaskAssignmentPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TaskAssignmentView view;
        [SerializeField] private WorkerSystem workerSystem;

        [Header("Data Source")]


        [SerializeField] private GameObject buildingGroup;
        [SerializeField] private List<ProductableBuilding> allBuildings;

        private ProductableBuilding currentSelectedBuilding;

        private void Awake()
        {
            view = GetComponentInChildren<TaskAssignmentView>();
            workerSystem = this.transform.root.GetComponent<WorkerSystem>();

            if (buildingGroup != null)
            {
                allBuildings = new List<ProductableBuilding>();

                allBuildings.Clear();

                buildingGroup.GetComponentsInChildren<ProductableBuilding>(true, allBuildings);
            }
        }










        private void Start()
        {
            view.OnBuildingClick += HandleBuildingSelected;
            view.OnWorkerClick += HandleWorkerSelected;

            view.UpdateStatusText("작업을 지시할 건물을 선택하세요.");
            view.RenderBuildingList(allBuildings);
            view.ClearWorkerList();
        }


















        private void HandleBuildingSelected(ProductableBuilding building)
        {
            currentSelectedBuilding = building;

            view.UpdateStatusText($"선택됨: {building.name}\n투입할 작업자를 선택하세요.");

            List<WorkerAI> idleWorkers = workerSystem.GetIdleWorkers();

            if (idleWorkers.Count == 0)
            {
                view.UpdateStatusText($"선택됨: {building.name}\n(가용한 작업자가 없습니다)");
                view.ClearWorkerList();
            }
            else
            {
                Debug.Log($"[Presenter] 뷰에게 작업자 {idleWorkers.Count}명 표시 요청");
                view.RenderWorkerList(idleWorkers);
            }
        }

        private void HandleWorkerSelected(WorkerAI worker)
        {
            if (currentSelectedBuilding == null) return;


            worker.AssignTask(currentSelectedBuilding);

            view.UpdateStatusText($"할당 완료!\n{worker.name} -> {currentSelectedBuilding}");

            currentSelectedBuilding = null;

            view.ClearWorkerList();
            view.UpdateStatusText("작업을 지시할 건물을 선택하세요.");
        }
    }
}
