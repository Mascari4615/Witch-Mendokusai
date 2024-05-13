using System;
using System.Collections.Generic;

namespace Mascari4615
{
	[Serializable]
	public struct QuestInfo
	{
		public QuestType Type;
		public List<GameEventType> GameEvents;
		public List<CriteriaInfo> Criterias;
		public List<EffectInfo> CompleteEffects;
		public List<EffectInfo> RewardEffects;
		public List<RewardInfo> Rewards;

		public float WorkTime;
		public bool AutoWork;
		public bool AutoComplete;
	}
}