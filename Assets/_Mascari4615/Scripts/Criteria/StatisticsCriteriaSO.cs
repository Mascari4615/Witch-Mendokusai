using System;
using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(StatisticsCriteriaSO), menuName = "Criteria/" + nameof(StatisticsCriteriaSO))]
	public class StatisticsCriteriaSO : NumCriteriaSO
	{
		[SerializeField] private StatisticsType statisticsType;

		public override void OnAfterDeserialize()
		{
			Data = new StatisticsCriteria(comparisonOperator, targetValue, statisticsType);
		}
	}
}