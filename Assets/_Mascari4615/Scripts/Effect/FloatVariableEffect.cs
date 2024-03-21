using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "FloatVariableEffect", menuName = "Effect/FloatVariableEffect")]
	public class FloatVariableEffect : Effect
	{
		[SerializeField] private CustomVariable<float> targetStat;
		[SerializeField] private float amount;

		public override void OnEffect()
		{
			targetStat.RuntimeValue += amount;
		}

		public override void Cancle()
		{
			targetStat.RuntimeValue -= amount;
		}
	}
}