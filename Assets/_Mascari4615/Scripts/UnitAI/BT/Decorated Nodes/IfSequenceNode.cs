using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-16 PM 11:20:37
// 작성자 : Rito

namespace Mascari4615
{
	public class IfSequenceNode : DecoratedCompositeNode
	{
		public IfSequenceNode(Func<bool> condition, params Node[] nodes)
			: base(condition, new SequenceNode(nodes)) { }
	}
}