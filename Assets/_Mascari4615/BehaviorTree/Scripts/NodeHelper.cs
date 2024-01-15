using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rito.BehaviorTree
{
    // Core에서
    // using static Rito.BehaviorTree.NodeHelper;
    public static class NodeHelper
    {
        public static SelectorNode Selector(params Node[] nodes) => new SelectorNode(nodes);
        public static SequenceNode Sequence(params Node[] nodes) => new SequenceNode(nodes);
        public static ParallelNode Parallel(params Node[] nodes) => new ParallelNode(nodes);

        public static ConditionNode Condition(Func<bool> condition) => new ConditionNode(condition);
        public static ConditionNode If(Func<bool> condition) => new ConditionNode(condition);
        public static ActionNode Action(Action action) => new ActionNode(action);

        public static IfActionNode IfAction(Func<bool> condition, Action action)
            => new IfActionNode(condition, action);
        public static IfElseActionNode IfElseAction(Func<bool> condition, Action ifAction, Action ifElseAction)
            => new IfElseActionNode(condition, ifAction, ifElseAction);

        public static WaitNode Wait(float duration) => new WaitNode(duration);
    }
}