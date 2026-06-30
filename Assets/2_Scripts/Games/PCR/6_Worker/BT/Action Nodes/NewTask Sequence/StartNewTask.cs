using UnityEngine;

namespace LUP.PCR
{
    public class StartNewTask : WorkerBlackboardNode
    {
        public StartNewTask(WorkerBlackboard bb) : base(bb) { }

        protected override NodeState OnUpdate()
        {
            if (OwnerAI.IsWorking)
            {
                return NodeState.SUCCESS;
            }

            ProductableBuilding building = GetData<ProductableBuilding>(BBKeys.AssignedWorkplace);

            if (building == null)
            {
                return NodeState.FAILURE;
            }

            if(building is ProductableBuilding productable)
            {
                building.EnterWorker();
            }

            if (WorkerComp != null)
            {
                Mover.Stop();
                WorkerComp.transform.position = building.WorkSpotWorldPos;

                WorkerComp.SetActionState(building.requiredAction);
            }

            OwnerAI.IsWorking = true;
            Debug.Log($"2-3. {building.placeName} 작업 시작 (모션: {building.requiredAction})");
            return NodeState.SUCCESS;
        }
    }
}
