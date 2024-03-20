using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIQuestSlot : UISlot
	{
		[SerializeField] private GameObject[] stateObjects;
		[SerializeField] private Image progress;

		public override void UpdateUI()
		{
			base.UpdateUI();

			if (Artifact)
			{
				Quest quest = Artifact as Quest;
			
				for (int i = 0; i < stateObjects.Length; i++)
					stateObjects[i].SetActive((int)quest.State == i);

				if (quest.State == QuestState.Working)
				{
					progress.fillAmount = DataManager.Instance.WorkManager.TryGetWorkByQuestID(quest.ID, out Work work) ? work.GetProgress() : 1;
				}
				else
				{
					progress.fillAmount = quest.GetProgress();
				}
			}
		}
	}
}