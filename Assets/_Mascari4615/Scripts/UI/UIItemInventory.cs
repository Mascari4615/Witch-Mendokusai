using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class UIItemInventory : UIDataBuffer<Item>
	{
		[SerializeField] private Transform filtersParent;
		[SerializeField] private ItemType filter = ItemType.None;

		public override bool Init()
		{
			if (base.Init() == false)
				return false;

			Inventory inventory = dataBuffer as Inventory;
			inventory.RegisterInventoryUI(this);

			foreach (UIItemSlot slot in Slots.Cast<UIItemSlot>())
				slot.SetInventory(inventory);

			if (filtersParent != null)
			{
				UISlot[] fillerButtons = filtersParent.GetComponentsInChildren<UISlot>(true);
				for (int i = 0; i < fillerButtons.Length; i++)
				{
					fillerButtons[i].SetSlotIndex(i);
					fillerButtons[i].SetSelectAction((UISlot slot) =>
					{
						SetFilter((ItemType)(slot.Index - 1));
					});
				}
			}

			return true;
		}

		public override void UpdateUI()
		{
			Inventory inventory = dataBuffer as Inventory;

			for (int i = 0; i < Slots.Count; i++)
			{
				UIItemSlot slot = Slots[i] as UIItemSlot;
				Item item = inventory.RuntimeItems.ElementAtOrDefault(i);

				if (item == null)
				{
					slot.SetArtifact(null);
					slot.UpdateUI();
					slot.gameObject.SetActive(dontShowEmptySlot == false);
				}
				else
				{
					ItemData itemData = item.Data;
					bool slotActive = (filter == ItemType.None) || (itemData.Type == filter);

					slot.SetArtifact(itemData, item.Amount);
					slot.UpdateUI();
					slot.gameObject.SetActive(slotActive);
				}
			}
		}

		public void UpdateSlotUI(int index, Item item)
		{
			if (item != null)
			{
				Slots[index].SetArtifact(item.Data, item.Amount);
			}
			else
			{
				Slots[index].SetArtifact(null, 1);
			}

			Slots[index].UpdateUI();
		}

		public void SetFilter(ItemType newFilter)
		{
			filter = newFilter;
			UpdateUI();
		}

		public void SetPriceType(PriceType priceType)
		{
			foreach (UIItemSlot slot in Slots.Cast<UIItemSlot>())
				slot.SetPriceType(priceType);
			UpdateUI();
		}
	}
}