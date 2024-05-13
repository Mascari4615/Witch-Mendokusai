using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class AddQuestEffect : IEffect
	{
		public void Apply(EffectInfo effectInfo)
		{
			QuestSO quest = effectInfo.Data as QuestSO;
			DataManager.Instance.QuestManager.AddQuest(new(quest));
		}
	}
}