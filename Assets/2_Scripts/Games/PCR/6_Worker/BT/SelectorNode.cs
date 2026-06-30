using System.Collections.Generic;
using UnityEngine;

namespace LUP.PCR
{
    public sealed class SelectorNode : BTNode
    {
        private List<BTNode> nodes = new List<BTNode>();
        public SelectorNode(List<BTNode> nodes)
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
                    case NodeState.SUCCESS:
                        return NodeState.SUCCESS;
                    case NodeState.FAILURE:
                        continue;
                }
            }
            return NodeState.FAILURE;
        }
    }

}
