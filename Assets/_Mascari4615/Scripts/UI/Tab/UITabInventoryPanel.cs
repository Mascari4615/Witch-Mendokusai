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
			Debug.Log(Equals(itemInventoryUI, null));

			itemInventoryUI.Init();
			foreach (UISlot slot in itemInventoryUI.Slots)
				slot.ToolTipTrigger.SetClickToolTip(clickToolTip);
		}

		public override void UpdateUI()
		{
			Debug.Log(Equals(itemInventoryUI, null));
			itemInventoryUI.UpdateUI();
		}
	}
}
