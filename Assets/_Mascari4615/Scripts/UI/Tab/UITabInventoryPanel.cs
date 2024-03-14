using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UITabInventoryPanel : UIPanel
	{
		private ToolTip clickToolTip;
		private UIItemInventory itemInventoryUI;

		[SerializeField] private Transform fillterButtonsParent;
		private UISlot[] fillerButtons;

		public override void Init()
		{
			clickToolTip = GetComponentInChildren<ToolTip>(true);
			itemInventoryUI = GetComponentInChildren<UIItemInventory>(true);
			fillerButtons = fillterButtonsParent.GetComponentsInChildren<UISlot>(true);

			itemInventoryUI.Init();
			foreach (UISlot slot in itemInventoryUI.Slots)
				slot.ToolTipTrigger.SetClickToolTip(clickToolTip);

			for (int i = 0; i < fillerButtons.Length; i++)
			{
				fillerButtons[i].SetSlotIndex(i);
				fillerButtons[i].SetSelectAction((UISlot slot) =>
				{
					itemInventoryUI.SetFillter((ItemType)(slot.Index - 1));
				});
			}
		}

		public override void UpdateUI()
		{
			itemInventoryUI.UpdateUI();
		}
	}
}
