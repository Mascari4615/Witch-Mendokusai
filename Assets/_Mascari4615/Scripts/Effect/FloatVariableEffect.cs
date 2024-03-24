using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "FloatVariableEffect", menuName = "Effect/FloatVariableEffect")]
	public class FloatVariableEffect : NumVariableEffect<float>
	{
		public override void Apply()
		{
			TargetStat.RuntimeValue = Calc(TargetStat.RuntimeValue, Value, ArithmeticOperator);
		}

		public override void Cancle()
		{
			TargetStat.RuntimeValue = Return(TargetStat.RuntimeValue, Value, ArithmeticOperator);
		}
	}
}