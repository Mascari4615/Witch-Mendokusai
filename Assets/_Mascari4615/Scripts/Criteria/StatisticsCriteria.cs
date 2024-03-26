using System;
using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	public class StatisticsCriteria : NumCriteria
	{
		public StatisticsType Type { get; private set; }

		public StatisticsCriteria(ComparisonOperator comparisonOperator, int targetValue, StatisticsType type) : base(comparisonOperator, targetValue)
		{
			Type = type;
		}

		public override int GetCurValue()
		{
			return SOManager.Instance.Statistics[Type];
		}
	}
}