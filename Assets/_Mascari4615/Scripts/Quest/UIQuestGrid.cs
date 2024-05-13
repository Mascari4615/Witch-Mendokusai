using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class UIQuestGrid : UIDataGrid<RuntimeQuest>
	{
		[SerializeField] private Transform filtersParent;
		private QuestType curFilter = QuestType.None;

		[SerializeField] private GameObject workButton;
		[SerializeField] private GameObject rewardButton;

		[SerializeField] private UIRewards rewardUI;
		[SerializeField] private UIQuestTooltipCriteria questCriteriaUI;

		[SerializeField] private GameObject noQuestInfo;

		[SerializeField] private bool resetFilterOnEnable = true;

		private RuntimeQuest CurQuest => Datas.Count > 0 ? Datas[CurSlotIndex] : null;

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
					fillerButtons[i].Init();
					fillerButtons[i].SetSlotIndex(i);
					fillerButtons[i].SetClickAction((slot) =>
					{
						QuestType newFilter = (QuestType)(slot.Index - 1);
						SetFilter(newFilter);
					});
				}
			}

			if (rewardUI != null)
				rewardUI.Init();

			if (questCriteriaUI != null)
				questCriteriaUI.Init();

			return true;
		}

		public override void UpdateUI()
		{
			int activeSlotCount = 0;

			foreach (UIQuestSlot slot in Slots.Cast<UIQuestSlot>())
			{
				RuntimeQuest quest = Datas.ElementAtOrDefault(slot.Index);

				if (quest == null)
				{
					slot.SetSlot(null);
					slot.gameObject.SetActive(dontShowEmptySlot == false);
				}
				else
				{
					bool slotActive = (curFilter == QuestType.None) || (quest.Type == curFilter);

					slot.SetQuestState(quest.State);
					slot.SetQuest(quest);
					slot.UpdateUI();

					if (quest.SO == null)
						slot.SetSlot(null, quest.Name, quest.Description);
					else
						slot.SetSlot(quest.SO.Sprite, quest.SO.Name, quest.SO.Description);

					slot.gameObject.SetActive(slotActive);
				}

				activeSlotCount += slot.gameObject.activeSelf ? 1 : 0;
			}

			if (noQuestInfo != null)
				noQuestInfo.SetActive(activeSlotCount == 0);

			if (CurSlot.DataSO)
			{
				if (workButton != null)
					workButton.SetActive(CurQuest.State == RuntimeQuestState.CanWork);
				if (rewardButton != null)
					rewardButton.SetActive(CurQuest.State == RuntimeQuestState.CanComplete);
			}
			else
			{
				if (workButton != null)
					workButton.SetActive(false);
				if (rewardButton != null)
					rewardButton.SetActive(false);
			}

			if (clickToolTip != null)
				clickToolTip.SetToolTipContent(CurSlot.Data);

			if (rewardUI != null)
				rewardUI.UpdateUI(CurQuest?.Rewards);

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
			if (CurQuest.State != RuntimeQuestState.CanComplete)
				return;

			CurQuest.Complete();
			SelectSlot(0);
		}

		public void StartQuestWork()
		{
			// TODO: 어떤 인형이 일을 할지
			if (CurQuest.State != RuntimeQuestState.CanWork)
				return;

			CurQuest.StartWork(0);
		}

		private void OnEnable()
		{
			if (resetFilterOnEnable)
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