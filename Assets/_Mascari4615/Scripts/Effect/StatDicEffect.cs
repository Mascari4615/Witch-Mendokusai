using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(StatEffect), menuName = "Effect/" + nameof(StatEffect))]
	public class StatEffect : NumEffect<int>
	{
		[field: SerializeField] public StatType Type { get; private set; }

		private Stat PlayerStat => PlayerController.Instance.PlayerObject.Stat;

		public override void Apply()
		{
			int newValue = (int)Calc(PlayerStat[Type], Value, ArithmeticOperator);
			PlayerStat[Type] = newValue;
		}

		public override void Cancle()
		{
			int newValue = (int)Return(PlayerStat[Type], Value, ArithmeticOperator);
			PlayerStat[Type] = newValue;
		}
	}
}