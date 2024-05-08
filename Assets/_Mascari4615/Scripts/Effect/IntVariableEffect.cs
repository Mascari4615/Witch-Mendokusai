using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class IntVariableEffect : IEffect
	{
		public void Apply(EffectInfo effectInfo)
		{
			IntVariable targetStat = effectInfo.Data as IntVariable;
			int value = effectInfo.Value;
			ArithmeticOperator arithmeticOperator = effectInfo.ArithmeticOperator;

			targetStat.RuntimeValue = (int)Arithmetic.Calc(targetStat.RuntimeValue, value, arithmeticOperator);
		}
	}
}