using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIQuestSlot : UISlot
	{
		[SerializeField] private GameObject[] questDataStateObjects;
		[SerializeField] private GameObject[] questStateObjects;
		[SerializeField] private Image progress;

		public override void UpdateUI()
		{
			base.UpdateUI();

			if (Artifact)
			{
				QuestData questData = Artifact as QuestData;
				for (int i = 0; i < questDataStateObjects.Length; i++)
					questDataStateObjects[i].SetActive((int)questData.State == i);
			}
		}

		public void SetQuestState(QuestState state)
		{
			for (int i = 0; i < questStateObjects.Length; i++)
				questStateObjects[i].SetActive((int)state == i);
		}

		public void SetProgress(float value)
		{
			progress.fillAmount = value;
		}
	}
}