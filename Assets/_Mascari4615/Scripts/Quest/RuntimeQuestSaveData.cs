using System;
using System.Collections.Generic;

namespace Mascari4615
{
	[Serializable]
	public struct RuntimeQuestSaveData
	{
		public Guid? Guid;
		public int DataID;
		public RuntimeQuestState State;
		public List<RuntimeCriteriaSaveData> Criterias;
		public List<RewardData> Rewards;
	}
}