using UnityEngine;

namespace LUP.PCR
{
    public class GoToEatingPlace : MoveActionBase
    {
        public GoToEatingPlace(WorkerBlackboard bb) : base(bb) { }
        protected override string GetBuildingKey() => BBKeys.Restaurant;
    }
}
