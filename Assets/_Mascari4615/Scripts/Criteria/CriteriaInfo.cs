using System;

namespace Mascari4615
{
	[Serializable]
	public struct CriteriaInfo
	{
		public CriteriaSO CriteriaSO;
		public int TargetValue;
		public ComparisonOperator ComparisonOperator;
		public bool JustOnce;
	}
}