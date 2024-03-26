using System;
using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	public class StatisticsCriteria : NumCriteria
	{
		public StatisticsType StatisticsType { get; private set; }

		public StatisticsCriteria(ComparisonOperator comparisonOperator, int targetValue, StatisticsType statisticsType) : base(comparisonOperator, targetValue)
		{
			StatisticsType = statisticsType;
		}

		public override bool Evaluate()
		{
			return Evaluate_(SOManager.Instance.Statistics[StatisticsType]);
		}

		public override float GetProgress()
		{
			return GetProgress_(SOManager.Instance.Statistics[StatisticsType]);
		}
	}
}