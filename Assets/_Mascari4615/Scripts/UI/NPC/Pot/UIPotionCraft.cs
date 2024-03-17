using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public class UIPotionCraft : UIPanel
	{
		[SerializeField] private UIItemSlot[] slots;
		[SerializeField] private UIItemSlot resultSlot;
		[SerializeField] private Inventory craftTableInventory;
		[SerializeField] private UIItemInventory itemInventoryUI;
		[SerializeField] private UIItemInventory potInventoryUI;

		public void TryCraft()
		{
			// Make Recipe
			List<int> recipeToList = new(slots.Length);
			foreach (UIItemSlot slot in slots)
			{
				if (slot.HasItem)
					recipeToList.Add(craftTableInventory.GetItem(slot.Index).Data.ID);
			}
			recipeToList.Sort();

			// Find Recipe
			string key = string.Join(',', recipeToList);
			if (DataManager.Instance.CraftDic.ContainsKey(key) == false)
				return;
			ItemData resultItemData = DataManager.Instance.ItemDic[DataManager.Instance.CraftDic[key]];

			// Make Potion
			if (resultSlot.HasItem)
			{
				if (resultSlot.Artifact.ID != resultItemData.ID)
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
				if (slot.HasItem)
					craftTableInventory.Remove(slot.Index);
			}
		}

		public override void Init()
		{
			itemInventoryUI.Init();
			potInventoryUI.Init();
			resultSlot.Init();
		}

		public override void UpdateUI()
		{
			itemInventoryUI.UpdateUI();
			potInventoryUI.UpdateUI();
			resultSlot.UpdateUI();
		}
	}
}