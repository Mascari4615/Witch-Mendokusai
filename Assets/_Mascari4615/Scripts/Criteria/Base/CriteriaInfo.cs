using System;
using static Mascari4615.SOHelper;

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

		public CriteriaInfo(CriteriaInfoSaveData criteriaInfoSaveData)
		{
			Type = criteriaInfoSaveData.Type;
			Data = Type switch
			{
				CriteriaType.ItemCount => GetItemData(criteriaInfoSaveData.DataID),
				CriteriaType.Stat => GetStatData(criteriaInfoSaveData.DataID),
				CriteriaType.Statistics => GetStatisticsData(criteriaInfoSaveData.DataID),
				_ => throw new ArgumentOutOfRangeException(),
			};
			ComparisonOperator = criteriaInfoSaveData.ComparisonOperator;
			Value = criteriaInfoSaveData.Value;
			JustOnce = criteriaInfoSaveData.JustOnce;
		}
	}
}