using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class UIItemInventory : MonoBehaviour
	{
		[SerializeField] private Inventory inventory;
		[HideInInspector] public List<ItemSlot> slots = new();
		[SerializeField] private Transform slotsParent;

		public void SetInventory(Inventory newInventory) => inventory = newInventory;

		private void Awake()
		{
			slots = slotsParent.GetComponentsInChildren<ItemSlot>().ToList();
			for (int i = 0; i < slots.Count; i++)
			{
				slots[i].SetSlotIndex(i);
				slots[i].SetInventory(inventory);
			}

			inventory.RegisterInventoryUI(this);
		}

		private void OnEnable() => UpdateUI();

		public void UpdateUI()
		{
			for (var i = 0; i < slots.Count; i++)
			{
				if (i < inventory.Capacity)
				{
					slots[i].UpdateUI(inventory.Items[i]?.Data, inventory.Items[i] != null ? inventory.Items[i].Amount : 1);
					slots[i].gameObject.SetActive(true);
				}
				else
				{
					slots[i].UpdateUI(null);
					slots[i].gameObject.SetActive(false);
				}
			}
		}

		public void UpdateSlotUI(int index, Item item)
		{
			slots[index].UpdateUI(item?.Data, item?.Amount ?? 1);
			UpdateUI();
		}
	}
}