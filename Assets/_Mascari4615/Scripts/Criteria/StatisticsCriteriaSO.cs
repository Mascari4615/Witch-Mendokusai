using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(StatisticsCriteriaSO), menuName = "Criteria/" + nameof(StatisticsCriteriaSO))]
	public class StatisticsCriteriaSO : CriteriaSO
	{
		[SerializeField] private StatisticsType statisticsType;

		public override Criteria CreateCriteria(CriteriaInfo criteriaInfo)
		{
			return new StatisticsCriteria(criteriaInfo, statisticsType);
		}
	}
}