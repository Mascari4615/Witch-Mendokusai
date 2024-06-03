using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class GameStatEffect : IEffect
	{
		private GameStat GameStat => SOManager.Instance.GameStat;

		public void Apply(EffectInfo effectInfo)
		{
			GameStatType Type = (effectInfo.Data as GameStatData).Type;
			int value = effectInfo.Value;
			ArithmeticOperator arithmeticOperator = effectInfo.ArithmeticOperator;

			int newValue = (int)Arithmetic.Calc(GameStat[Type], value, arithmeticOperator);
			GameStat[Type] = newValue;
		}
	}
}