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

		public void SetToolTip(ToolTip toolTip)
		{
			foreach (UIQuestSlot slot in questSlots)
			{
				slot.ToolTipTrigger.SetClickToolTip(toolTip);
				slot.SetClickAction((slot) => slot.ToolTipTrigger.ClickToolTip.gameObject.SetActive(true));
			}
		}

		public override void UpdateUI()
		{
			foreach (UIQuestSlot slot in questSlots)
				slot.UpdateUI();
		}

		public override void OnOpen()
		{
			// 스크롤 위치 초기화
			content.anchoredPosition = Vector2.zero;
		}
	}
}