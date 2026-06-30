using UnityEngine;

namespace LUP.PCR
{
    public class GoToNewTaskLocation : MoveActionBase
    {
        public GoToNewTaskLocation(WorkerBlackboard bb) : base(bb) { }

        protected override string GetBuildingKey() => BBKeys.AssignedWorkplace;

        protected override NodeState OnUpdate()
        {
            if (!OwnerAI.HasTask)
            {
                OwnerAI.HasTask = true;
            }

            if (targetPlace == null || !targetPlace.IsWorkRequested)
            {
                if (targetPlace != null)
                {
                    targetPlace.SetWorker(null);
                }

                if (Mover != null)
                {
                    Mover.Stop();
                }

                BB.Remove(BBKeys.AssignedWorkplace);
                OwnerAI.HasTask = false;

                return NodeState.FAILURE;
            }

            return base.OnUpdate();
        }
    }
}
