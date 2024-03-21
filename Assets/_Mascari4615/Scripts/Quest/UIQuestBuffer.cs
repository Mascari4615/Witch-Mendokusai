using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class UIQuestBuffer : UIDataBuffer<Quest>
	{
		// [SerializeField] private Transform filtersParent;
		// [SerializeField] private ItemType filter = ItemType.None;

		[SerializeField] private GameObject workButton;
		[SerializeField] private GameObject rewardButton;

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

		public override bool Init()
		{
			if (base.Init() == false)
				return false;

			QuestBuffer questBuffer = dataBuffer as QuestBuffer;
			// questBuffer.RegisterInventoryUI(this);

			// foreach (UIQuestSlot slot in Slots.Cast<UIQuestSlot>())
			//	slot.SetInventory(questBuffer);

			// if (filtersParent != null)
			// {
			// 	UISlot[] fillerButtons = filtersParent.GetComponentsInChildren<UISlot>(true);
			// 	for (int i = 0; i < fillerButtons.Length; i++)
			// 	{
			// 		fillerButtons[i].SetSlotIndex(i);
			// 		fillerButtons[i].SetSelectAction((slot) => {SetFilter((ItemType)(slot.Index - 1));});
			// 	}
			// }

			return true;
		}

		public override void UpdateUI()
		{
			QuestBuffer questBuffer = dataBuffer as QuestBuffer;

			for (int i = 0; i < Slots.Count; i++)
			{
				UIQuestSlot slot = Slots[i] as UIQuestSlot;
				Quest quest = questBuffer.RuntimeItems.ElementAtOrDefault(i);

				if (quest == null)
				{
					slot.SetArtifact(null);
					slot.gameObject.SetActive(dontShowEmptySlot == false);
				}
				else
				{
					QuestData questData = quest.Data;
					// bool slotActive = (filter == ItemType.None) || (itemData.Type == filter);

					slot.SetQuestState(quest.State);
					slot.SetProgress(quest.GetProgress());
					slot.SetArtifact(questData);
					// slot.gameObject.SetActive(slotActive);
					slot.gameObject.SetActive(true);
				}
			}

			if (CurSlot.Artifact)
			{	
				Quest curQuest = SOManager.Instance.QuestBuffer.RuntimeItems[CurSlotIndex];
				workButton.SetActive(curQuest.State == QuestState.NeedWorkToComplete);
				rewardButton.SetActive(curQuest.State == QuestState.Completed);
			}
			else
			{
				workButton.SetActive(false);
				rewardButton.SetActive(false);
			}

			if (clickToolTip != null)
				clickToolTip.SetToolTipContent(CurSlot.Artifact);
		}

		// public void SetFilter(ItemType newFilter)
		// {
		// 	filter = newFilter;
		// 	UpdateUI();
		// }

		public void GetReward()
		{
			Quest quest = SOManager.Instance.QuestBuffer.RuntimeItems[CurSlotIndex];
			if (quest.State != QuestState.Completed)
				return;
			
			quest.GetReward();
			SelectSlot(0);
			UpdateUI();
		}

		public void Work()
		{
			// TODO: 어떤 인형이 일을 할지
			Quest quest = SOManager.Instance.QuestBuffer.RuntimeItems[CurSlotIndex];
			if (quest.State != QuestState.NeedWorkToComplete)
				return;

			quest.StartWork(0);
		}
	}
}