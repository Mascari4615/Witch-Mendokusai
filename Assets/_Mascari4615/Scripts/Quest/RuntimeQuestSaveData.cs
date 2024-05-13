using System;
using System.Collections.Generic;

namespace Mascari4615
{
	[Serializable]
	public struct RuntimeQuestSaveData
	{
		public Guid? Guid;
		public RuntimeQuestState State;

		public int SO_ID;

		public QuestType Type;
		public List<GameEventType> GameEvents;
		public List<RuntimeCriteriaSaveData> Criterias;
		public List<EffectInfoData> CompleteEffects;
		public List<EffectInfoData> RewardEffects;
		public List<RewardInfoData> Rewards;

		public float WorkTime;
		public bool AutoWork;
		public bool AutoComplete;
	}
}