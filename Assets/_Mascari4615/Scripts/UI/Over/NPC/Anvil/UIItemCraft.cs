using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using static Mascari4615.SOHelper;

namespace Mascari4615
{
	public class UIItemCraft : UIPanel
	{
		[SerializeField] private UIItemSlot[] slots;
		[SerializeField] private UIItemSlot resultSlot;
		[SerializeField] private Inventory craftTableInventory;
		[SerializeField] private UIItemGrid itemInventoryUI;
		[SerializeField] private UIItemGrid anvilInventoryUI;

		public void TryCraft()
		{
			// Make Recipe
			List<int> recipeToList = new(slots.Length);
			foreach (UIItemSlot slot in slots)
			{
				if (slot.DataSO)
					recipeToList.Add(craftTableInventory.GetItem(slot.Index).Data.ID);
			}
			recipeToList.Sort();

			// Find Recipe
			string key = string.Join(',', recipeToList);
			if (DataManager.Instance.CraftDic.ContainsKey(key) == false)
				return;
			ItemData resultItemData = GetItemData(DataManager.Instance.CraftDic[key]);

			// Make Potion
			if (resultSlot.DataSO)
			{
				if (resultSlot.DataSO.ID != resultItemData.ID)
					return;
				else
					craftTableInventory.SetItemAmount(resultSlot.Index, craftTableInventory.GetItem(resultSlot.Index).Amount + 1);
			}
			else
			{
				Item newItem = new(Guid.NewGuid(), resultItemData);
				craftTableInventory.SetItem(resultSlot.Index, newItem);
			}

			// Actual Remove
			foreach (UIItemSlot slot in slots)
			{
				if (slot.DataSO)
					craftTableInventory.Remove(slot.Index);
			}
		}

		public override void Init()
		{
			itemInventoryUI.Init();
			anvilInventoryUI.Init();
			resultSlot.Init();
		}

		public override void UpdateUI()
		{
			itemInventoryUI.UpdateUI();
			anvilInventoryUI.UpdateUI();
			resultSlot.UpdateUI();
		}
	}
}