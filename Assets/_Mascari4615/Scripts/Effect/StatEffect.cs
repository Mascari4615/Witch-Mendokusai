using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class StatEffect : IEffect
	{
		private Stat PlayerStat => Player.Instance.Stat;

		public void Apply(EffectInfo effectInfo)
		{
			StatType Type = (effectInfo.Data as StatData).Type;
			int value = effectInfo.Value;
			ArithmeticOperator arithmeticOperator = effectInfo.ArithmeticOperator;

			int newValue = (int)Arithmetic.Calc(PlayerStat[Type], value, arithmeticOperator);
			PlayerStat[Type] = newValue;
		}
	}
}