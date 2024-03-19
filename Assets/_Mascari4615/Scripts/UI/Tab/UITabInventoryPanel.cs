using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UITabInventoryPanel : UIPanel
	{
		private UIItemInventory itemInventoryUI;

		public override void Init()
		{
			itemInventoryUI = GetComponentInChildren<UIItemInventory>(true);
			itemInventoryUI.Init();
		}

		public override void UpdateUI()
		{
			itemInventoryUI.UpdateUI();
		}
	}
}
