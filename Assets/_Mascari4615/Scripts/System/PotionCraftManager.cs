using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public class PotionCraftManager : MonoBehaviour
	{
		[SerializeField] private UIItemSlot[] slots;
		[SerializeField] private UIItemSlot resultSlot;
		[SerializeField] private Inventory craftTableInventory;

		private void Awake()
		{
			craftTableInventory.LoadSaveItems(new List<InventorySlotData>());
		}

		public void TryCraft()
		{
			if (resultSlot.HasItem)
				return;

			var recipeToList = new List<int>();

			foreach (var slot in slots)
			{
				if (!slot.HasItem)
					continue;

				recipeToList.Add(craftTableInventory.GetItem(slot.Index).Data.ID);
			}

			recipeToList.Sort();
			var key = String.Join(',', recipeToList);
			if (!DataManager.Instance.CraftDic.ContainsKey(key))
				return;
			var newItem = new Item(Guid.NewGuid(), DataManager.Instance.ItemDic[DataManager.Instance.CraftDic[key]]);
			craftTableInventory.SetItem(0, newItem);

			Debug.Log(newItem);
		}
	}
}