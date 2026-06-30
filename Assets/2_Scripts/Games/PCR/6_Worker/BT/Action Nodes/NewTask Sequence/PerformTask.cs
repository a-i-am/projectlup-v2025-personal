using UnityEngine;

namespace LUP.PCR
{
    public class PerformTask : WorkerBlackboardNode
    {
        public PerformTask(WorkerBlackboard bb) : base(bb) { }
        protected override NodeState OnUpdate()
        {
            StructureBase workingPlace = GetData<StructureBase>(BBKeys.AssignedWorkplace);

            if (workingPlace == null)
            {
                OwnerAI.StopWorkAndResetState();
                return NodeState.FAILURE;
            }


            if (!workingPlace.IsWorkRequested)
            {
                workingPlace.ExitWorker();
                OwnerAI.StopWorkAndResetState();
                return NodeState.FAILURE;
            }


            return NodeState.RUNNING;
        }


    }
}
