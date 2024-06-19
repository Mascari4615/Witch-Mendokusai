using System;

namespace Mascari4615
{
	[Serializable]
	public struct DungeonConstraintEffectInfo
	{
		// 몬스터 타입
		public UnitAffiliation Affiliation;
		public UnitStatType StatType;
		public int Value;
	}
}