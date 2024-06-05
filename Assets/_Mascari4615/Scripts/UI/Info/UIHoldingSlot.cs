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

		public bool IsHolding => HoldingItem != null;

		public Item HoldingItem { get; private set; }

		public void HoldSlot(UIItemSlot targetSlot)
		{
			Inventory inventory = SOManager.Instance.ItemInventory;
			
			if (targetSlot == null || inventory.GetItem(targetSlot.Index) == null)
				return;

			HoldingItem = inventory.GetItem(targetSlot.Index);
			thisSlot.SetSlot(HoldingItem.Data, HoldingItem.Amount);

			inventory.SetItem(targetSlot.Index, null);

			targetSlot.Inventory.UpdateSlot(targetSlot.Index);
		}

		public void SwapSlot(UIItemSlot targetSlot)
		{
			if (targetSlot == null || HoldingItem == null)
				return;

			Inventory inventory = SOManager.Instance.ItemInventory;

			Item slotItem = inventory.GetItem(targetSlot.Index);

			if (slotItem.Data.ID == HoldingItem.Data.ID)
			{
				int maxAmount = slotItem.MaxAmount;
				int sum = slotItem.Amount + HoldingItem.Amount;

				if (sum <= maxAmount)
				{
					slotItem.SetAmount(sum);

					HoldingItem.SetAmount(0);
					HoldingItem = null;
				}
				else
				{
					slotItem.SetAmount(maxAmount);
					HoldingItem.SetAmount(sum - maxAmount);
				}
			}
			else
			{
				inventory.SetItem(targetSlot.Index, HoldingItem);
				HoldingItem = slotItem;
				thisSlot.SetSlot(HoldingItem.Data, HoldingItem.Amount);
			}

			targetSlot.Inventory.UpdateSlot(targetSlot.Index);
		}

		public Item DropSlot()
		{
			Item result = HoldingItem;
			HoldingItem = null;

			Debug.Log("DropSlot: " + result);
			return result;
		}

		private void Update()
		{
			canvasGroup.alpha = IsHolding ? 1 : 0;
		
			if (IsHolding)
				transform.position = Input.mousePosition;
		}
	}
}