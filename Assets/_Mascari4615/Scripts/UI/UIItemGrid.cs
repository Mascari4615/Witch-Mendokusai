using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class UIItemGrid : UIDataGrid<Item>
	{
		[SerializeField] private Transform filtersParent;
		[SerializeField] private ItemType filter = ItemType.None;

		public override bool Init()
		{
			if (base.Init() == false)
				return false;

			Inventory inventory = DataBufferSO as Inventory;
			inventory.RegisterUI(this);

			foreach (UIItemSlot slot in Slots.Cast<UIItemSlot>())
				slot.SetUIItemGrid(this);

			if (filtersParent != null)
			{
				UISlot[] fillerButtons = filtersParent.GetComponentsInChildren<UISlot>(true);
				for (int i = 0; i < fillerButtons.Length; i++)
				{
					fillerButtons[i].Init();
					fillerButtons[i].SetSlotIndex(i);
					fillerButtons[i].SetClickAction((slot) => { SetFilter((ItemType)(slot.Index - 1)); });
				}
			}

			return true;
		}

		public override void UpdateUI()
		{
			Inventory inventory = DataBufferSO as Inventory;

			for (int i = 0; i < Slots.Count; i++)
			{
				if (inventory.Capacity <= i)
				{
					Slots[i].gameObject.SetActive(false);
					continue;
				}

				UIItemSlot slot = Slots[i] as UIItemSlot;
				Item item = inventory.Datas.ElementAtOrDefault(i);

				slot.canDrag = filter == ItemType.None;

				if (item == null)
				{
					slot.SetSlot(null);
					slot.gameObject.SetActive((dontShowEmptySlot == false) && (filter == ItemType.None));
					slot.SetDisable(false);
				}
				else
				{
					ItemData itemData = item.Data;
					bool slotDisable = (filter != ItemType.None) && (itemData.Type != filter);

					slot.SetSlot(itemData, item.Amount);
					slot.gameObject.SetActive(slotDisable == false);
					// slot.SetDisable(slotDisable);
				}
			}
		}

		public void UpdateSlotUI(int index, Item item)
		{
			if (item != null)
			{
				Slots[index].SetSlot(item.Data, item.Amount);
			}
			else
			{
				Slots[index].SetSlot(null);
			}
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