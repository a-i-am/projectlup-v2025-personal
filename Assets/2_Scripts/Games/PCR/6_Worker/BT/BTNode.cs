namespace LUP.PCR
{
    public abstract class BTNode
    {
        public BTNode()
        {

        }

        public enum NodeState
        {
            RUNNING,
            SUCCESS,
            FAILURE,
        }
        protected NodeState state = NodeState.FAILURE;
        private bool isStarted = false;

        public NodeState Evaluate()
        {
            if (!isStarted)
            {
                OnStart();
                isStarted = true;
            }

            state = OnUpdate();

            if (state != NodeState.RUNNING)
            {
                isStarted = false;
            }

            return state;
        }
        protected virtual void OnStart() { }
        protected abstract NodeState OnUpdate();
    }
}
