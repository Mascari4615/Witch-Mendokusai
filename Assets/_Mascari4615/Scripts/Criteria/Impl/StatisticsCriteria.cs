using UnityEngine;

namespace Mascari4615
{
	public class StatisticsCriteria : NumCriteria
	{
		public StatisticsType Type { get; private set; }

		public StatisticsCriteria(CriteriaInfo criteriaInfo) : base(criteriaInfo)
		{
			Debug.Log(criteriaInfo);
			Debug.Log(criteriaInfo.Data);
			Debug.Log((criteriaInfo.Data as StatisticsData).Type);
			Type = (criteriaInfo.Data as StatisticsData).Type;
		}

		public override int GetCurValue()
		{
			return SOManager.Instance.Statistics[Type];
		}
	}
}