using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIChapter : UIPanel
	{
		private UIQuestSlot[] questSlots;
		[SerializeField] private RectTransform content;

		public override void Init()
		{
			questSlots = GetComponentsInChildren<UIQuestSlot>(true);

			foreach (UIQuestSlot slot in questSlots)
				slot.Init();
		}

		public void SetToolTip(ToolTip toolTip, UIQuestToolTip questToolTip)
		{
			foreach (UIQuestSlot slot in questSlots)
			{
				slot.ToolTipTrigger.SetClickToolTip(toolTip);
				slot.SetClickAction((slot) =>
				{
					slot.ToolTipTrigger.ClickToolTip.gameObject.SetActive(true);

					RuntimeQuest quest = DataManager.Instance.QuestManager.GetQuest((slot as UIQuestSlot).DataSO as QuestSO);
					questToolTip.SetQuest(quest);
					questToolTip.UpdateUI();
				});
			}
		}

		public override void OnOpen()
		{
			// 스크롤 위치 초기화
			content.anchoredPosition = Vector2.zero;
		}

		public override void UpdateUI()
		{
			foreach (UIQuestSlot slot in questSlots)
			{
				RuntimeQuest runtimeQuest = DataManager.Instance.QuestManager.GetQuest(slot.DataSO as QuestSO);

				// HACK:
				slot.SetDisable(false);

				// 진행 중
				if (runtimeQuest != null)
				{
					slot.SetRuntimeQuestState(runtimeQuest.State);
					slot.SetQuest(runtimeQuest);
				}
				else
				{
					QuestSO questData = slot.DataSO as QuestSO;
					slot.SetDisable(questData.State == QuestState.Locked);
				}

				slot.UpdateUI();
			}
		}
	}
}