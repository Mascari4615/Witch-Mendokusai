using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	/// <summary> 자식들 리턴에 관계 없이 모두 순회하는 노드 </summary>
	public class ParallelNode : CompositeNode
	{
		List<State> childrenLeftToExecute = new List<State>();

		public ParallelNode(params Node[] nodes) : base(nodes) { }

		//public override void OnStart()
		// {
			//childrenLeftToExecute.Clear();
			//ChildList.ForEach(a => { childrenLeftToExecute.Add(State.Running); });
		// }

		public override State OnUpdate()
		{
			foreach (var node in ChildList)
			{
				node.OnUpdate();
			}
			return State.Success;
		}
	}
}