using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "IntVariableEffect", menuName = "Effect/IntVariableEffect")]
	public class IntVariableEffect : NumVariableEffect<int>
	{
		public override void OnEffect()
		{
			TargetStat.RuntimeValue = (int)Calc(TargetStat.RuntimeValue, Value, ArithmeticOperator);
		}

		public override void Cancle()
		{
			TargetStat.RuntimeValue = (int)Return(TargetStat.RuntimeValue, Value, ArithmeticOperator);
		}
	}
}