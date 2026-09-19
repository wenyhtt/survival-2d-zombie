using System;

namespace AI.BehaviorTree
{
    public class ConditionNode : Node
    {
        private Func<bool> condition;

        public ConditionNode(Func<bool> condition)
        {
            this.condition = condition;
        }

        public override NodeState Evaluate()
        {
            if (condition == null)
            {
                State = NodeState.Failure;
                return State;
            }

            State = condition.Invoke() ? NodeState.Success : NodeState.Failure;
            return State;
        }
    }
}
