using UnityEngine;

namespace LUP.PCR
{
    public class GoToWorkerStation : MoveActionBase
    {
        public GoToWorkerStation(WorkerBlackboard bb) : base(bb) { }
        protected override string GetBuildingKey() => BBKeys.WorkerStation;
    }
}
