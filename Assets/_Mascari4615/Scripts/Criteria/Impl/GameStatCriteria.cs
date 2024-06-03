using UnityEngine;

namespace Mascari4615
{
	public class GameStatCriteria : NumCriteria
	{
		public GameStatType Type { get; private set; }

		public GameStatCriteria(CriteriaInfo criteriaInfo) : base(criteriaInfo)
		{
			Type = (criteriaInfo.Data as GameStatData).Type;
		}

		public override int GetCurValue()
		{
			return SOManager.Instance.GameStat[Type];
		}
	}
}