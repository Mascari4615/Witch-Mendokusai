using System;
using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	public class StatCriteria : NumCriteria
	{
		public StatType Type { get; private set; }

		public StatCriteria(ComparisonOperator comparisonOperator, int targetValue, StatType type) : base(comparisonOperator, targetValue)
		{
			Type = type;
		}

		private Stat PlayerStat => PlayerController.Instance.PlayerObject.Stat;

		public override int GetCurValue()
		{
			return PlayerStat[Type];
		}
	}
}