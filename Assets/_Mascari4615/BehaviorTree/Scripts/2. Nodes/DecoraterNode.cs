using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rito.BehaviorTree
{
	/// <summary> ������ ���� ��� </summary>
	public abstract class DecoraterNode : Node
	{
		public Node child;
	}
}