using System.Collections.Generic;

namespace LUP.PCR
{
    public sealed class SequenceNode : BTNode
    {
        private List<BTNode> nodes = new List<BTNode>();
        public SequenceNode(List<BTNode> nodes)
        {
            this.nodes = nodes;
        }
        protected override NodeState OnUpdate()
        {
            foreach (BTNode node in nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                    case NodeState.FAILURE:
                        return NodeState.FAILURE;
                    case NodeState.SUCCESS:
                        continue;
                }
            }
            return NodeState.SUCCESS;
        }
    }

}
