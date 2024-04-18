using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIChapter : UIPanel
	{
		private UIQuestSlot[] questSlots;

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
	}
}