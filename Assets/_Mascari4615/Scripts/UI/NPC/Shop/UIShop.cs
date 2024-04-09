using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class UIShop : UINPCPanel
	{
		private UIItemDataGrid shopInventoryUI;
		private UIItemGrid itemInventoryUI;

		public override void Init()
		{
			base.Init();
			
			shopInventoryUI = GetComponentInChildren<UIItemDataGrid>(true);
			itemInventoryUI = GetComponentInChildren<UIItemGrid>(true);

			shopInventoryUI.Init();
			shopInventoryUI.SetPriceType(PriceType.Buy);
			foreach (UISlot slot in shopInventoryUI.Slots)
			{
				slot.SetClickAction((slot) =>
				{
					shopInventoryUI.SelectSlot(slot.Index);
					BuyItem(slot.DataSO.ID);
				});
			}

			itemInventoryUI.Init();
			itemInventoryUI.SetPriceType(PriceType.Sell);
			foreach (UISlot slot in itemInventoryUI.Slots)
			{
				slot.SetClickAction((slot) =>
				{
					itemInventoryUI.SelectSlot(slot.Index);
					SellItem(slot.Index);
				});
			}
		}

		public override void SetNPC(NPCObject npc)
		{
			shopInventoryUI.SetDataBuffer(npc.Data.ItemDataBuffers[0]);
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
				SOManager.Instance.ItemInventory.Add(itemData);
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
	}
}