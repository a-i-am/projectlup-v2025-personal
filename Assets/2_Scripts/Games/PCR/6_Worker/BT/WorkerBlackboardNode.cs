using UnityEngine;

namespace LUP.PCR
{


    public abstract class WorkerBlackboardNode : BTNode
    {
        protected readonly WorkerBlackboard BB;
        private WorkerAI ownerAI;
        private Worker workerComp;
        private UnitMover mover;
        private NodeState? lastReturnState = null;

        protected WorkerBlackboardNode(WorkerBlackboard blackboard) : base()
        {
            BB = blackboard;
        }
        protected NodeState ReturnAndLog(NodeState newState, string message)
        {
            if (lastReturnState != newState)
            {
                Debug.Log($"[{this.GetType().Name}] {message} (상태: {newState})");
                lastReturnState = newState;
            }

            return newState;
        }
        protected WorkerAI OwnerAI
        {
            get
            {
                if (ownerAI == null)
                {
                    BB.TryGetValue(BBKeys.OwnerAI, out ownerAI);
                }
                return ownerAI;
            }
        }
        protected Worker WorkerComp
        {
            get
            {
                if (workerComp == null)
                {
                    BB.TryGetValue(BBKeys.Self, out workerComp);
                }
                return workerComp;
            }
        }
        protected UnitMover Mover
        {
            get
            {
                if (mover == null)
                {
                    BB.TryGetValue(BBKeys.UnitMover, out mover);
                }
                return mover;
            }
        }



        protected T GetData<T>(string key) => BB.GetValue<T>(key);
        protected void SetData<T>(string key, T value) => BB.SetValue(key, value);
        protected bool HasData(string key) => BB.HasKey(key);
    }
}

