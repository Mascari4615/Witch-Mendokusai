using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIQuestSlot : UISlot
	{
		[SerializeField] private GameObject completeEffect;

		public override void UpdateUI()
		{
			base.UpdateUI();

			if (Artifact)
			{
				Quest quest = Artifact as Quest;
				completeEffect.SetActive(quest.IsCompleted);
			}
		}
	}
}