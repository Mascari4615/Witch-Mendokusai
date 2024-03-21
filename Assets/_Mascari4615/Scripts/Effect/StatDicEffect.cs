using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(StatDicEffect), menuName = "Effect/StatDicEffect")]
	public class StatDicEffect : Effect
	{
		[SerializeField] private StatDictionary statDictionary;
		[SerializeField] private string stat;
		[SerializeField] private int value;

		public override void OnEffect()
		{
			statDictionary.SetStat(stat, value);
		}

		public override void Cancle()
		{
			statDictionary.SetStat(stat, -value);
		}
	}
}