using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class UIQuestBuffer : UIDataBuffer<Quest>
	{
		[SerializeField] private Transform filtersParent;
		[SerializeField] private QuestType curFilter = QuestType.None;

		[SerializeField] private GameObject workButton;
		[SerializeField] private GameObject rewardButton;

		public QuestBuffer QuestBuffer => dataBuffer as QuestBuffer;
		public Quest CurQuest => QuestBuffer.RuntimeItems[CurSlotIndex];

		public override bool Init()
		{
			// 이미 한 번 Init했다면 Return
			if (base.Init() == false)
				return false;

			// 필터 버튼 초기화
			if (filtersParent != null)
			{
				UISlot[] fillerButtons = filtersParent.GetComponentsInChildren<UISlot>(true);
				for (int i = 0; i < fillerButtons.Length; i++)
				{
					fillerButtons[i].SetSlotIndex(i);
					fillerButtons[i].SetSelectAction((slot) =>
					{
						QuestType newFilter = (QuestType)(slot.Index - 1);
						curFilter = newFilter;
						UpdateUI();
					});
				}
			}

			return true;
		}

		public override void UpdateUI()
		{
			foreach (UIQuestSlot slot in Slots.Cast<UIQuestSlot>())
			{
				Quest quest = QuestBuffer.RuntimeItems.ElementAtOrDefault(slot.Index);

				if (quest == null)
				{
					slot.SetArtifact(null);
					slot.gameObject.SetActive(dontShowEmptySlot == false);
				}
				else
				{
					// // TEMP
					// foreach (RuntimeCriteria criteriaData in quest.Criterias)
					// {
					// 	if (criteriaData.Criteria is IntCriteria intCriteria)
					// 		Debug.Log($"{intCriteria.IntVariable.name} {intCriteria.IntVariable.RuntimeValue}");
					// 	else if (criteriaData.Criteria is ItemCountCriteria itemCountCriteria)
					// 		Debug.Log($"{itemCountCriteria.ItemID}");
					// }

					QuestData questData = quest.GetData();
					bool slotActive = (curFilter == QuestType.None) || (questData.Type == curFilter);

					slot.SetQuestState(quest.State);
					slot.SetProgress(quest.GetProgress());
					slot.SetArtifact(questData);
					slot.gameObject.SetActive(slotActive);
				}
			}
			
			if (CurSlot.Artifact)
			{
				workButton.SetActive(CurQuest.State == QuestState.NeedWorkToComplete);
				rewardButton.SetActive(CurQuest.State == QuestState.Completed);
			}
			else
			{
				workButton.SetActive(false);
				rewardButton.SetActive(false);
			}

			if (clickToolTip != null)
				clickToolTip.SetToolTipContent(CurSlot.Artifact);
		}

		public void GetReward()
		{
			if (CurQuest.State != QuestState.Completed)
				return;

			CurQuest.GetReward();
			SelectSlot(0);
			UpdateUI();
		}

		public void Work()
		{
			// TODO: 어떤 인형이 일을 할지
			if (CurQuest.State != QuestState.NeedWorkToComplete)
				return;

			CurQuest.StartWork(0);
		}

		private void OnEnable() => StartCoroutine(UpdateLoop());
		private void OnDisable() => StopAllCoroutines();
		public IEnumerator UpdateLoop()
		{
			WaitForSeconds wait = new(.1f);
			while (true)
			{
				UpdateUI();
				yield return wait;
			}
		}
	}
}