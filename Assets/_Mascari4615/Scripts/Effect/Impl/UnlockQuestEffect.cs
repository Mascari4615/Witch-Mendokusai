using UnityEngine;

namespace Mascari4615
{
	public class UnlockQuestEffect : IEffect
	{
		public void Apply(EffectInfo effectInfo)
		{
			(effectInfo.Data as QuestSO).Unlock();
		}
	}
}