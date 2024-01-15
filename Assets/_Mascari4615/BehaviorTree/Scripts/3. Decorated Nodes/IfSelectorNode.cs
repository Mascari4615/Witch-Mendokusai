using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-16 PM 11:23:12
// 작성자 : Rito

namespace Rito.BehaviorTree
{
	public class IfSelectorNode : DecoratedCompositeNode
	{
		public IfSelectorNode(Func<bool> condition, params Node[] nodes)
			: base(condition, new SelectorNode(nodes)) { }

		public override void OnEnd()
		{
			throw new NotImplementedException();
		}

		public override void OnStart()
		{
			throw new NotImplementedException();
		}
	}
}