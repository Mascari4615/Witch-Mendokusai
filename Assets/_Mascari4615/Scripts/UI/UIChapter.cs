using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIChapter : MonoBehaviour
	{
		private UIQuestSlot[] questSlots;
		
		private void Awake()
		{
			questSlots = GetComponentsInChildren<UIQuestSlot>(true);
		}

		public void Init()
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