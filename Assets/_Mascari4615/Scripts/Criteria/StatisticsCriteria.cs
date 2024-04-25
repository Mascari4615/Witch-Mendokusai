namespace Mascari4615
{
	public class StatisticsCriteria : NumCriteria
	{
		public StatisticsType Type { get; private set; }

		public StatisticsCriteria(CriteriaInfo criteriaInfo, StatisticsType type) : base(criteriaInfo)
		{
			Type = type;
		}

		public override int GetCurValue()
		{
			return SOManager.Instance.Statistics[Type];
		}
	}
}