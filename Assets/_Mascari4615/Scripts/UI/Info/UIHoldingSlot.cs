using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIHoldingSlot : Singleton<UIHoldingSlot>
	{
		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private UISlot thisSlot;

		private Item holdingItem;
		public bool IsHolding => holdingItem != null;

		public void HoldSlot(UIItemSlot targetSlot)
		{
			Inventory inventory = SOManager.Instance.ItemInventory;
			Item targetItem = inventory.GetItem(targetSlot.Index);

			if (targetSlot == null || targetItem == null)
				return;

			holdingItem = targetItem;

			inventory.SetItem(targetSlot.Index, null);

			targetSlot.Inventory.UpdateSlot(targetSlot.Index);
		}

		public void HoldSlotHalf(UIItemSlot targetSlot)
		{
			Inventory inventory = SOManager.Instance.ItemInventory;
			Item targetItem = inventory.GetItem(targetSlot.Index);

			if (targetSlot == null || inventory.GetItem(targetSlot.Index) == null)
				return;

			if (targetItem.Amount == 1)
			{
				HoldSlot(targetSlot);
				return;
			}

			int halfAmount = targetItem.Amount / 2;
			holdingItem = new Item(new(), targetItem.Data, halfAmount);

			targetItem.SetAmount(targetItem.Amount - halfAmount);

			targetSlot.Inventory.UpdateSlot(targetSlot.Index);
		}

		public void SwapSlot(UIItemSlot targetSlot)
		{
			if (targetSlot == null || holdingItem == null)
				return;

			Inventory inventory = SOManager.Instance.ItemInventory;

			Item slotItem = inventory.GetItem(targetSlot.Index);

			if (slotItem.Data.ID == holdingItem.Data.ID)
			{
				int maxAmount = slotItem.MaxAmount;
				int sum = slotItem.Amount + holdingItem.Amount;

				if (sum <= maxAmount)
				{
					slotItem.SetAmount(sum);

					holdingItem.SetAmount(0);
					holdingItem = null;
				}
				else
				{
					slotItem.SetAmount(maxAmount);
					holdingItem.SetAmount(sum - maxAmount);
				}
			}
			else
			{
				inventory.SetItem(targetSlot.Index, holdingItem);
				holdingItem = slotItem;
			}

			targetSlot.Inventory.UpdateSlot(targetSlot.Index);
		}

		public Item DropSlot()
		{
			Item dropItem = holdingItem;
			holdingItem = null;

			// Debug.Log("DropSlot: " + dropItem);
			return dropItem;
		}

		public Item DropSlotOne()
		{
			if (holdingItem == null)
				return null;

			Item newItem = new(new(), holdingItem.Data, 1);
			holdingItem.SetAmount(holdingItem.Amount - 1);

			if (holdingItem.Amount <= 0)
				holdingItem = null;

			return newItem;
		}

		private void Update()
		{
			UpdateUI();
		}

		private void UpdateUI()
		{
			canvasGroup.alpha = IsHolding ? 1 : 0;
		
			if (IsHolding)
			{
				transform.position = Input.mousePosition;
				thisSlot.SetSlot(holdingItem.Data, holdingItem.Amount);
			}
		}
	}
}