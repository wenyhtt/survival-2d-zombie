using System;

namespace AI.BehaviorTree
{
    public class ActionNode : Node
    {
        private Func<NodeState> action;

        public ActionNode(Func<NodeState> action)
        {
            this.action = action;
        }

        public override NodeState Evaluate()
        {
            State = action?.Invoke() ?? NodeState.Failure;
            return State;
        }
    }
}
