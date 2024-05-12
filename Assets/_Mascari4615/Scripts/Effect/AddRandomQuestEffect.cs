using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class AddRandomQuestEffect : IEffect
	{
		public void Apply(EffectInfo effectInfo)
		{
			DataBufferSO<Quest> questDataBuffer = effectInfo.Data as DataBufferSO<Quest>;
			Quest randomQuest = questDataBuffer.Datas[Random.Range(0, questDataBuffer.Datas.Count)];
			DataManager.Instance.QuestManager.AddQuest(new(randomQuest));
		}
	}
}