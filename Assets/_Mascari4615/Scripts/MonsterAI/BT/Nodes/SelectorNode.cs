using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	/// <summary> �ڽĵ��� ��ȸ�ϸ� true�� �� �ϳ��� �����ϴ� ��� </summary>
	public class SelectorNode : CompositeNode
	{
		public SelectorNode(params Node[] nodes) : base(nodes) { }
		protected int current;

		public override State OnUpdate()
		{
			for (; current < ChildList.Count; ++current)
			{
				Node child = ChildList[current];
				switch (child.Update())
				{
					case State.Running:
						return State.Running;
					case State.Success:
						return State.Success;
					case State.Failure:
						continue;
				}
			}

			current = 0;
			return State.Failure;
		}
	}
}