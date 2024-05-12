using System;

namespace Mascari4615
{
	[Serializable]
	public struct CriteriaInfoSaveData
	{
		public CriteriaType Type;
		public int DataID;
		public ComparisonOperator ComparisonOperator;
		public int Value;
		public bool JustOnce;

		public CriteriaInfoSaveData(CriteriaInfo criteriaInfo)
		{
			Type = criteriaInfo.Type;
			DataID = criteriaInfo.Data.ID;
			ComparisonOperator = criteriaInfo.ComparisonOperator;
			Value = criteriaInfo.Value;
			JustOnce = criteriaInfo.JustOnce;
		}
	}
}