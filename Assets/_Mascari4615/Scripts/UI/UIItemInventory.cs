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
		[HideInInspector] public List<UIItemSlot> slots = new();
		[SerializeField] private Transform slotsParent;

		public void SetInventory(Inventory newInventory) => inventory = newInventory;

		private void Awake()
		{
			slots = slotsParent.GetComponentsInChildren<UIItemSlot>().ToList();
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
					slots[i].Init(inventory.Items[i]?.Data, inventory.Items[i] != null ? inventory.Items[i].Amount : 1);
					slots[i].gameObject.SetActive(true);
				}
				else
				{
					slots[i].Init(null);
					slots[i].gameObject.SetActive(false);
				}
			}
		}

		public void UpdateSlotUI(int index, Item item)
		{
			slots[index].Init(item?.Data, item?.Amount ?? 1);
			UpdateUI();
		}
	}
}