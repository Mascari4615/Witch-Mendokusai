using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rito.BehaviorTree
{
	public class WaitNode : Node
	{
		private float duration;
		private float t;

		public WaitNode(float duration)
		{
			this.duration = duration;
		}

		public override void OnEnd()
		{
			throw new NotImplementedException();
		}

		public override void OnStart()
		{
			t = duration;
		}

		public override State OnUpdate()
		{
			if (t < duration)
				t += Time.deltaTime;

			bool timeout = t > duration;
			return timeout ? State.Success : State.Failure;
		}
	}
}