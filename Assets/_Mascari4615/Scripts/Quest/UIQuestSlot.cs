using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIQuestSlot : UISlot
	{
		[SerializeField] private Image[] criteriaObjects;
		[SerializeField] private GameObject[] questStateObjects;
		[SerializeField] private GameObject[] runtimeQuestStateObjects;
		[SerializeField] private Image progress;
		[SerializeField] private TextMeshProUGUI progressText;

		public override void UpdateUI()
		{
			base.UpdateUI();

			if (DataSO)
			{
				QuestSO questData = DataSO as QuestSO;
				SetQuestState(questData.State);
			}
		}

		public void SetQuestState(QuestState state)
		{
			for (int i = 0; i < questStateObjects.Length; i++)
				questStateObjects[i].SetActive((int)state == i);
		}

		public void SetRuntimeQuestState(RuntimeQuestState state)
		{
			for (int i = 0; i < runtimeQuestStateObjects.Length; i++)
				runtimeQuestStateObjects[i].SetActive((int)state == i);
		}

		public void SetQuest(RuntimeQuest quest)
		{
			for (int i = 0; i < criteriaObjects.Length; i++)
			{
				if (i < quest.Criterias.Count)
				{
					criteriaObjects[i].color = quest.Criterias[i].IsCompleted ? Color.green : Color.red;
					criteriaObjects[i].gameObject.SetActive(true);
				}
				else
				{
					criteriaObjects[i].gameObject.SetActive(false);
				}
			}

			progress.fillAmount = quest.GetProgress();
			progressText.text = quest.GetProgressText();
		}
	}
}