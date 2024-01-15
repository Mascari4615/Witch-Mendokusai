using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "IntVariableEffect", menuName = "Effect/IntVariableEffect")]
	public class IntVariableEffect : Effect
	{
		[SerializeField] private CustomVariable<int> targetStat;
		[SerializeField] private int amount;

		public override void OnEquip()
		{
			targetStat.RuntimeValue += amount;
		}

		public override void OnRemove()
		{
			targetStat.RuntimeValue -= amount;
		}
	}
}