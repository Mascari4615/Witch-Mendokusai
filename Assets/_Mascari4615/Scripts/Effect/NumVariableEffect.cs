using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public abstract class NumVariableEffect<T> : Effect
	{
		[field: SerializeField] public CustomVariable<T> TargetStat { get; private set; }
		[field: SerializeField] public ArithmeticOperator ArithmeticOperator { get; private set; }
		[field: SerializeField] public T Value { get; private set; }
		private float originValue;

		protected float Calc(float a, float b, ArithmeticOperator arithmeticOperator)
		{
			originValue = a;
			return arithmeticOperator switch
			{
				ArithmeticOperator.Set => b,
				ArithmeticOperator.Add => a + b,
				ArithmeticOperator.Subtract => a - b,
				ArithmeticOperator.Multiply => a * b,
				ArithmeticOperator.Divide => a / b,
				ArithmeticOperator.Remainder => a % b,
				ArithmeticOperator.Power => (float)Mathf.Pow(a, b),
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		protected float Return(float a, float b, ArithmeticOperator arithmeticOperator)
		{
			return arithmeticOperator switch
			{
				ArithmeticOperator.Set => b,
				ArithmeticOperator.Add => a - b,
				ArithmeticOperator.Subtract => a + b,
				ArithmeticOperator.Multiply => a / b,
				ArithmeticOperator.Divide => a * b,
				ArithmeticOperator.Remainder => originValue,
				ArithmeticOperator.Power => (float)Mathf.Pow(a, 1 / b),
				_ => throw new ArgumentOutOfRangeException(),
			};
		}
	}
}