using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mascari4615
{
	[CreateAssetMenu(fileName = "E_" + nameof(AddRandomQuestEffect), menuName = "Effect/" + nameof(AddRandomQuestEffect))]
	public class AddRandomQuestEffect : Effect
	{
		[SerializeField] private DataBufferSO<QuestData> questDataBuffer;

		public override void Apply()
		{
			QuestData randomQuest = questDataBuffer.Datas[Random.Range(0, questDataBuffer.Datas.Count)];
			DataManager.Instance.QuestManager.AddQuest(new(randomQuest));
		}

		public override void Cancle()
		{
		}
	}
}