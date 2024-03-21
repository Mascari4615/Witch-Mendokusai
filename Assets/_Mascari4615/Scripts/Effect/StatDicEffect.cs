using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(StatDicEffect), menuName = "Effect/StatDicEffect")]
	public class StatDicEffect : NumVariableEffect<int>
	{
		[SerializeField] private StatDictionary statDictionary;
		[field: SerializeField] public Stat Stat { get; private set; }

		public override void OnEffect()
		{
			int originValue = statDictionary.GetStat(Stat);
			int newValue = (int)Calc(originValue, Value, ArithmeticOperator);
			statDictionary.SetStat(Stat, newValue);
		}

		public override void Cancle()
		{
			int originValue = statDictionary.GetStat(Stat);
			int newValue = (int)Return(originValue, Value, ArithmeticOperator);
			statDictionary.SetStat(Stat, newValue);
		}
	}
}