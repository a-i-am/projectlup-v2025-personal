using UnityEngine;

namespace LUP.PCR
{
    public class IsNewTaskChecker : WorkerBlackboardNode
    {
        public IsNewTaskChecker(WorkerBlackboard bb) : base(bb) { }
        protected override NodeState OnUpdate()
        {
            BuildingBase building = GetData<BuildingBase>(BBKeys.AssignedWorkplace);

            return building != null
                ? ReturnAndLog(NodeState.SUCCESS, "2-1. 예약된 작업이 있습니다.")
                : ReturnAndLog(NodeState.FAILURE, "2-1. 할당된 작업이 없습니다.");
        }
    }
}
