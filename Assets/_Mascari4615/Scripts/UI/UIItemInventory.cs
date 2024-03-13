using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Mascari4615
{
	public enum UIItemInventoryFillter
	{
		All,
		Item,
		Equipment,
	}

	public class UIItemInventory : MonoBehaviour
	{
		public List<UIItemSlot> Slots { get; private set; } = new();

		[SerializeField] private Inventory inventory;
		[SerializeField] private Transform slotsParent;
		[SerializeField] private UIItemInventoryFillter fillter = UIItemInventoryFillter.All;

		public void SetInventory(Inventory newInventory) => inventory = newInventory;

		private bool isInit = false;

		private void Awake()
		{
			Init();
		}

		public void Init()
		{
			if (isInit)
				return;

			Slots = slotsParent.GetComponentsInChildren<UIItemSlot>(true).ToList();
			for (int i = 0; i < Slots.Count; i++)
			{
				Slots[i].SetSlotIndex(i);
				Slots[i].SetInventory(inventory);
			}

			inventory.RegisterInventoryUI(this);

			isInit = true;
		}

		private void OnEnable() => UpdateUI();

		public void UpdateUI()
		{
			switch (fillter)
			{
				case UIItemInventoryFillter.All:
					for (int i = 0; i < Slots.Count; i++)
					{
						if (i < inventory.Capacity)
						{
							Slots[i].SetArtifact(inventory.Items[i]?.Data, inventory.Items[i] != null ? inventory.Items[i].Amount : 1);
							Slots[i].gameObject.SetActive(true);
						}
						else
						{
							Slots[i].SetArtifact(null);
							Slots[i].gameObject.SetActive(false);
						}
					}
					break;
				case UIItemInventoryFillter.Item:
					break;
				case UIItemInventoryFillter.Equipment:
					for (int i = 0; i < Slots.Count; i++)
					{
						if ((i < inventory.Capacity) && (inventory.Items[i]?.Data is EquipmentData))
						{
							Slots[i].SetArtifact(inventory.Items[i]?.Data, 1);
							Slots[i].gameObject.SetActive(true);
						}
						else
						{
							Slots[i].SetArtifact(null);
							Slots[i].gameObject.SetActive(false);
						}
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void UpdateSlotUI(int index, Item item)
		{
			Slots[index].SetArtifact(item?.Data, item?.Amount ?? 1);
			UpdateUI();
		}
	}
}