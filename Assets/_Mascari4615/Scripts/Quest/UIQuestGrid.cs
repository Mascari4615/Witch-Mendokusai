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

		private UIQuestToolTip questToolTip;

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

			questToolTip = GetComponentInChildren<UIQuestToolTip>(true);

			if (questToolTip != null)
				questToolTip.Init();

			return true;
		}

		public override void UpdateUI()
		{
			Init();

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

					slot.SetRuntimeQuestState(quest.State);
					slot.SetQuest(quest);
					slot.UpdateUI();

					if (quest.SO == null)
						slot.SetSlot(null, quest.Name, quest.Description);
					else
						slot.SetSlot(quest.SO);

					slot.gameObject.SetActive(slotActive);
				}

				activeSlotCount += slot.gameObject.activeSelf ? 1 : 0;
			}

			if (noQuestInfo != null)
				noQuestInfo.SetActive(activeSlotCount == 0);

			if (clickToolTip != null)
				clickToolTip.SetToolTipContent(CurSlot.Data);

			if (questToolTip != null)
			{
				questToolTip.SetQuest(CurQuest);
				questToolTip.UpdateUI();
			}
		}

		public void SetFilter(QuestType filter)
		{
			curFilter = filter;
			UpdateUI();
		}

		private void OnEnable() => TimeManager.Instance.RegisterCallback(UpdateUI);
		private void OnDisable() => TimeManager.Instance.RemoveCallback(UpdateUI);
	}
}