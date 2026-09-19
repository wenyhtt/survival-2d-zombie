using System.Collections.Generic;

namespace AI.BehaviorTree
{
    public class Sequence : Node
    {
        private List<Node> nodes = new List<Node>();

        public Sequence(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (var node in nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Failure:
                        State = NodeState.Failure;
                        return State;
                    case NodeState.Success:
                        continue;
                    case NodeState.Running:
                        anyChildIsRunning = true;
                        continue; // We continue to evaluate other nodes if needed, or you could return running immediately. Standard Sequence returns running immediately. Let's return running immediately.
                }
                
                // If it's running, we should return running immediately.
                if (anyChildIsRunning)
                {
                    State = NodeState.Running;
                    return State;
                }
            }
            
            State = anyChildIsRunning ? NodeState.Running : NodeState.Success;
            return State;
        }
    }
}
