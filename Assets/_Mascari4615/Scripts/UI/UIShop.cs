using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class UIShop : UIPanel
	{
		private UIItemInventory itemInventoryUI;
		private UIItemInventory shopInventoryUI;

		public override void Init()
		{
			UIItemInventory[] inventoryUIs = GetComponentsInChildren<UIItemInventory>(true);
			shopInventoryUI = inventoryUIs[0];
			itemInventoryUI = inventoryUIs[1];

			shopInventoryUI.Init();
			itemInventoryUI.Init();
		}

		public override void UpdateUI()
		{
			shopInventoryUI.UpdateUI();
			itemInventoryUI.UpdateUI();
		}
	}
}