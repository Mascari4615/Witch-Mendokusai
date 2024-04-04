using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class UIShop : UINPCPanel
	{
		private UIItemInventory itemInventoryUI;
		private UIItemInventory shopInventoryUI;

		public override void Init()
		{
			UIItemInventory[] inventoryUIs = GetComponentsInChildren<UIItemInventory>(true);
			shopInventoryUI = inventoryUIs[0];
			itemInventoryUI = inventoryUIs[1];

			shopInventoryUI.Init();
			shopInventoryUI.SetPriceType(PriceType.Buy);
			foreach (UISlot slot in shopInventoryUI.Slots)
			{
				slot.SetSelectAction((slot) =>
				{
					shopInventoryUI.SelectSlot(slot.Index);
					BuyItem(slot.Artifact.ID);
				});
			}

			itemInventoryUI.Init();
			itemInventoryUI.SetPriceType(PriceType.Sell);
			foreach (UISlot slot in itemInventoryUI.Slots)
			{
				slot.SetSelectAction((slot) =>
				{
					itemInventoryUI.SelectSlot(slot.Index);
					SellItem(slot.Index);
				});
			}
		}

		public override void UpdateUI()
		{
			shopInventoryUI.UpdateUI();
			itemInventoryUI.UpdateUI();
		}

		public void BuyItem(int itemID)
		{
			ItemData itemData = GetItemData(itemID);
			if (itemData.PurchasePrice <= SOManager.Instance.Nyang.RuntimeValue)
			{
				SOManager.Instance.Nyang.RuntimeValue -= itemData.PurchasePrice;
				SOManager.Instance.ItemDataBuffer.Add(itemData);
				UpdateUI();
			}
		}

		public void SellItem(int slotIndex)
		{
			Item item = SOManager.Instance.ItemInventory.GetItem(slotIndex);
			if (item != null)
			{
				ItemData itemData = item.Data;
				SOManager.Instance.Nyang.RuntimeValue += itemData.SalePrice;
				SOManager.Instance.ItemInventory.Remove(slotIndex);
				UpdateUI();
			}
		}

		public override void SetNPC(NPCObject npc)
		{
		}
	}
}