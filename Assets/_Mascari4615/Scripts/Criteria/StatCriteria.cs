using System;
using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	public class StatCriteria : NumCriteria
	{
		public StatType StatType { get; private set; }

		public StatCriteria(ComparisonOperator comparisonOperator, int targetValue, StatType statType) : base(comparisonOperator, targetValue)
		{
			StatType = statType;
		}

		private Stat PlayerStat => PlayerController.Instance.PlayerObject.Stat;

		public override bool Evaluate()
		{
			return Evaluate_(PlayerStat[StatType]);
		}

		public override float GetProgress()
		{
			return GetProgress_(PlayerStat[StatType]);
		}
	}
}