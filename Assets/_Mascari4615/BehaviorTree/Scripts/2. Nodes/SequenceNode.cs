using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Rito.BehaviorTree
{
	/// <summary> 자식들 중 false가 나올 때까지 연속으로 순회하는 노드 </summary>
	public class SequenceNode : CompositeNode
	{
		public SequenceNode(params Node[] nodes) : base(nodes) { }
		protected int current;

		public override void OnEnd()
		{
		}

		public override void OnStart()
		{
			current = 0;
		}

		public override State OnUpdate()
		{
			for (; current < ChildList.Count; ++current)
			{
				Node node = ChildList[current];
				State result = node.OnUpdate();
				if (result == State.Failure)
					return State.Failure;
			}
			return State.Success;
		}
	}
}