using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mascari4615
{
	[CreateAssetMenu(fileName = "E_" + nameof(AddRandomQuestEffect), menuName = "Effect/" + nameof(AddRandomQuestEffect))]
	public class AddRandomQuestEffect : Effect
	{
		[SerializeField] private DataBuffer<QuestData> questDataBuffer;

		public override void Apply()
		{
			QuestData randomQuest = questDataBuffer.RuntimeItems[Random.Range(0, questDataBuffer.RuntimeItems.Count)];
			DataManager.Instance.QuestManager.AddQuest(new(randomQuest));
		}

		public override void Cancle()
		{
		}
	}
}