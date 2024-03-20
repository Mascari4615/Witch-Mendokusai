using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Mascari4615
{
	public class UIQuestBuffer : UIDataBuffer<Quest>
	{
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

		public override void UpdateUI()
		{
			for (int i = 0; i < Slots.Count; i++)
			{
				UIQuestSlot slot = Slots[i] as UIQuestSlot;
				Quest quest = dataBuffer.RuntimeItems.ElementAtOrDefault(i);

				if (quest == null)
				{
					slot.SetArtifact(null);
					slot.gameObject.SetActive(dontShowEmptySlot == false);
				}
				else
				{
					bool slotActive = quest.State > QuestState.Locked;

					slot.SetArtifact(quest);
					slot.gameObject.SetActive(slotActive);
				}
			}

			if (CurSlot.Artifact)
			{
				Quest curQuest = CurSlot.Artifact as Quest;
				workButton.SetActive(curQuest.State == QuestState.NeedWorkToComplete);
				rewardButton.SetActive(curQuest.State == QuestState.Completed);
			}
		}

		public void GetReward()
		{
			Quest quest = Slots[CurSlotIndex].Artifact as Quest;

			if (quest.State != QuestState.Completed)
				return;
			quest.GetReward();
			UpdateUI();
		}

		public void Work()
		{
			// TODO: 어떤 인형이 일을 할지
			Quest quest = Slots[CurSlotIndex].Artifact as Quest;

			if (quest.State != QuestState.NeedWorkToComplete)
				return;

			Work work = new(0, WorkType.CompleteQuest, quest.ID, 5);
			DataManager.Instance.WorkManager.AddWork(work);
			quest.StartWork();
		}
	}
}