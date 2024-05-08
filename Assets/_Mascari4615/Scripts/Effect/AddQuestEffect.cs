using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class AddQuestEffect : IEffect
	{
		public void Apply(EffectInfo effectInfo)
		{
			QuestData quest = effectInfo.Data as QuestData;
			DataManager.Instance.QuestManager.AddQuest(new(quest));
		}
	}
}