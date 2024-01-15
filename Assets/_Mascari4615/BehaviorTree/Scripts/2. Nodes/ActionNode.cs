using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rito.BehaviorTree
{
	/// <summary> 행동 수행 노드 </summary>
	public class ActionNode : Node
	{
		public Action Action { get; protected set; }
		public ActionNode(Action action)
		{
			Action = action;
		}

		public override void OnEnd()
		{
			throw new NotImplementedException();
		}

		public override void OnStart()
		{
			throw new NotImplementedException();
		}

		public override State OnUpdate()
		{
			Action();
			return State.Success;
		}

		// Action <=> ActionNode 타입 캐스팅
		public static implicit operator ActionNode(Action action) => new ActionNode(action);
		public static implicit operator Action(ActionNode action) => new Action(action.Action);
	}
}