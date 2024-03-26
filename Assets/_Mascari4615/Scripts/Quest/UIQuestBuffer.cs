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
		private QuestType curFilter = QuestType.None;

		[SerializeField] private GameObject workButton;
		[SerializeField] private GameObject rewardButton;

		[SerializeField] private UIQuestTooltipReward questRewardUI;
		[SerializeField] private UIQuestTooltipCriteria questCriteriaUI;

		private QuestBuffer QuestBuffer => dataBuffer as QuestBuffer;
		private Quest CurQuest => QuestBuffer.RuntimeItems.Count > 0 ? QuestBuffer.RuntimeItems[CurSlotIndex] : null;

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
						SetFilter(newFilter);
					});
				}
			}

			if (questRewardUI != null)
				questRewardUI.Init();

			if (questCriteriaUI != null)
				questCriteriaUI.Init();

			return true;
		}

		public override void UpdateUI()
		{
			foreach (UIQuestSlot slot in Slots.Cast<UIQuestSlot>())
			{
				Quest quest = QuestBuffer.RuntimeItems.ElementAtOrDefault(slot.Index);

				if (quest == null)
				{
					slot.SetSlot(null);
					slot.gameObject.SetActive(dontShowEmptySlot == false);
				}
				else
				{
					QuestData questData = quest.GetData();
					bool slotActive = (curFilter == QuestType.None) || (questData.Type == curFilter);

					slot.SetQuestState(quest.State);
					slot.SetProgress(quest.GetProgress());
					slot.SetQuest(quest);

					slot.SetSlot(questData);
					slot.gameObject.SetActive(slotActive);
				}
			}

			if (CurSlot.Artifact)
			{
				workButton.SetActive(CurQuest.State == QuestState.CanWork);
				rewardButton.SetActive(CurQuest.State == QuestState.CanComplete);
			}
			else
			{
				workButton.SetActive(false);
				rewardButton.SetActive(false);
			}

			if (clickToolTip != null)
				clickToolTip.SetToolTipContent(CurSlot.Data);

			if (questRewardUI != null)
				questRewardUI.SetReward(CurQuest);

			if (questCriteriaUI != null)
				questCriteriaUI.SetCriteria(CurQuest);
		}

		public void SetFilter(QuestType filter)
		{
			curFilter = filter;
			UpdateUI();
		}

		public void CompleteQuest()
		{
			if (CurQuest.State != QuestState.CanComplete)
				return;

			CurQuest.Complete();
			SelectSlot(0);
		}

		public void StartQuestWork()
		{
			// TODO: 어떤 인형이 일을 할지
			if (CurQuest.State != QuestState.CanWork)
				return;

			CurQuest.StartWork(0);
		}

		private void OnEnable()
		{
			SetFilter(QuestType.None);
			StartCoroutine(UpdateLoop());
		}
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