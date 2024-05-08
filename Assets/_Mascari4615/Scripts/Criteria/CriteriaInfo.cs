using System;

namespace Mascari4615
{
	[Serializable]
	public struct CriteriaInfo
	{
		public CriteriaType Type;
		public DataSO Data;
		public ComparisonOperator ComparisonOperator;
		public int Value;
		public bool JustOnce;
	}
}