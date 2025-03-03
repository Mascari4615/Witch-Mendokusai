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
				CriteriaType.UnitStat => GetUnitStatData(criteriaInfoSaveData.DataID),
				CriteriaType.GameStat => GetGameStatData(criteriaInfoSaveData.DataID),
				_ => throw new ArgumentOutOfRangeException(),
			};
			ComparisonOperator = criteriaInfoSaveData.ComparisonOperator;
			Value = criteriaInfoSaveData.Value;
			JustOnce = criteriaInfoSaveData.JustOnce;
		}
	}
}