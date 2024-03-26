using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(AddQuestEffect), menuName = "Effect/" + nameof(AddQuestEffect))]
	public class AddQuestEffect : Effect
	{
		[SerializeField] private QuestData quest;

		public override void Apply()
		{
			DataManager.Instance.QuestManager.AddQuest(new(quest));
		}

		public override void Cancle()
		{
		}
	}
}