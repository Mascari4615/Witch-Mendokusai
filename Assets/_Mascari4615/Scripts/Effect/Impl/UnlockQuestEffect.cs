using UnityEngine;

namespace Mascari4615
{
	public class UnlockQuestEffect : IEffect
	{
		public void Apply(EffectInfo effectInfo)
		{
			QuestManager.Instance.UnlockQuest(effectInfo.Data as QuestSO);
		}
	}
}