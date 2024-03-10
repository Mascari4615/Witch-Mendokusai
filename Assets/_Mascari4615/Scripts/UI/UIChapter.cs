using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIChapter : MonoBehaviour
	{
		private UIQuestSlot[] questSlots;

		public void Init()
		{
			questSlots = GetComponentsInChildren<UIQuestSlot>(true);

			foreach (var slot in questSlots)
				slot.Init();
		}

		public void UpdateUI()
		{
			foreach (var slot in questSlots)
				slot.UpdateUI();
		}

		public void SetToolTip(ToolTip toolTip)
		{
			foreach (var slot in questSlots)
				slot.toolTipTrigger.targetToolTip = toolTip;
		}
	}
}