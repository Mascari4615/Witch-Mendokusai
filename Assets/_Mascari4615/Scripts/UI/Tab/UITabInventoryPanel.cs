using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UITabInventoryPanel : UIPanel
	{
		private ToolTip clickToolTip;
		private UIItemInventory itemInventoryUI;

		public override void Init()
		{
			clickToolTip = GetComponentInChildren<ToolTip>(true);
			itemInventoryUI = GetComponentInChildren<UIItemInventory>(true);

			itemInventoryUI.Init();
			foreach (UISlot slot in itemInventoryUI.Slots)
				slot.ToolTipTrigger.SetClickToolTip(clickToolTip);
		}

		public override void UpdateUI(int[] someData = null)
		{
			itemInventoryUI.UpdateUI();
		}
	}
}
