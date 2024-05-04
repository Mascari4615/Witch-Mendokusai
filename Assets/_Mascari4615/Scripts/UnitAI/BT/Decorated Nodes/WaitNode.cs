using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class WaitNode : Node
	{
		private readonly float duration;
		private float t;

		public WaitNode(float duration)
		{
			this.duration = duration;
		}

		public override State OnUpdate()
		{
			t += Time.deltaTime;
			bool timeout = t >= duration;

			if (timeout)
			{
				t = 0;
				return State.Success;
			}
			else
			{
				return State.Failure;
			}
		}
	}
}