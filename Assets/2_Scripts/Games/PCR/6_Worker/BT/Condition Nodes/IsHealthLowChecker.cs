using UnityEngine;

namespace LUP.PCR
{
    public class IsHealthLowChecker : WorkerBlackboardNode
    {
       public IsHealthLowChecker(WorkerBlackboard bb) : base(bb) { }
       protected override NodeState OnUpdate()
        {
            float currentHunger = GetData<float>(BBKeys.Hunger);
            bool isHungry = currentHunger >= HungerRules.HungryThreshold;

            OwnerAI.IsHunger = isHungry;

            if (isHungry)
            {
                return ReturnAndLog(NodeState.SUCCESS, "배고픔 감지됨!");
            }
            else
            {
                return ReturnAndLog(NodeState.FAILURE, "아직 배고프지 않음.");
            }
        }
    }
}

