using UnityEngine;

namespace LUP.PCR
{
    public class PauseCurrentTask : WorkerBlackboardNode
    {
        public PauseCurrentTask(WorkerBlackboard bb) : base(bb) { }

        protected override NodeState OnUpdate()
        {
            BuildingBase building = GetData<BuildingBase>(BBKeys.AssignedWorkplace);

            if (building != null)
            {
                building.ExitWorker();
                OwnerAI.StopWorkAndResetState();
                Debug.Log($"1-2. 배고픔으로 인해 {building.buildingName} 작업을 취소했습니다.");
            }

            return NodeState.SUCCESS;
        }
    }
}

