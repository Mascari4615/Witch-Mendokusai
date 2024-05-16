using UnityEngine;

namespace Mascari4615
{
	public class UnlockQuestEffect : IEffect
	{
		public void Apply(EffectInfo effectInfo)
		{
			DataManager.Instance.QuestState[(effectInfo.Data as QuestSO).ID] = QuestState.Unlocked;
		}
	}
}