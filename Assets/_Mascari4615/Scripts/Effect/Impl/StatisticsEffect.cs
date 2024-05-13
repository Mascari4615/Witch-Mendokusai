using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class StatisticsEffect : IEffect
	{
		private Statistics Statistics => SOManager.Instance.Statistics;

		public void Apply(EffectInfo effectInfo)
		{
			StatisticsType Type = (effectInfo.Data as StatisticsData).Type;
			int value = effectInfo.Value;
			ArithmeticOperator arithmeticOperator = effectInfo.ArithmeticOperator;

			int newValue = (int)Arithmetic.Calc(Statistics[Type], value, arithmeticOperator);
			Statistics[Type] = newValue;
		}
	}
}