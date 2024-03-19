
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Mascari4615
{
	public enum PriceType
	{
		Buy,
		Sell
	}
	
	public class UIItemSlot : UISlot, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
	{
		protected TextMeshProUGUI priceText;
	
		public Inventory Inventory { get; private set; }

		public bool onlyOneItem = false;
		public bool canPlayerSetItem = true;
		public bool canDrag = true;
		private PriceType priceType = PriceType.Buy;

		public void SetInventory(Inventory inventory) => Inventory = inventory;
		public void SetPriceType(PriceType priceType)
		{
			this.priceType = priceType;
			UpdateUI();
		}

		public override bool Init()
		{
			if (base.Init() == false)
				return false;

			priceText = transform.Find("[Text] Price").GetComponent<TextMeshProUGUI>();

			return true;
		}

		public override void UpdateUI()
		{
			base.UpdateUI();
			
			if (Artifact)
			{
				ItemData itemData = Artifact as ItemData;
				priceText.text = (priceType == PriceType.Buy) ? itemData.PurchasePrice.ToString() : itemData.SalePrice.ToString();
			}
			else
			{
				priceText.text = string.Empty;
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!canDrag)
				return;

			if (!Artifact)
				return;

			DragSlot.instance.SetSlot(this);
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!canDrag)
				return;

			if (!Artifact)
				return;

			DragSlot.instance.transform.position = eventData.position;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (!canDrag)
				return;

			DragSlot.instance.SetColor(0);
			DragSlot.instance.SetSlot(null);
		}

		// DragSlot이 위에 떨어졌을 때
		public void OnDrop(PointerEventData eventData)
		{
			if (!canDrag)
				return;

			if (!DragSlot.instance.isHoldingSomething)
				return;

			if (!canPlayerSetItem)
				return;

			SwapSlot();
		}

		private void SwapSlot()
		{
			if (onlyOneItem)
			{
				if (!Artifact)
				{
					// DragSlot.HoldingSlot의 Item에서 하나만 가져오기 (빼오기)
				}
				else
				{
					// slot.SpecialThing을 아이템 인벤토리애 넣기
					// DragSlot.HoldingSlot의 Item에서 하나만 가져오기 
				}
			}
			else
			{
				// ChangeSlot
			}

			UIItemSlot slotA = this;
			UIItemSlot slotB = DragSlot.instance.HoldingSlot;

			if (slotA == slotB)
			{
				Debug.Log("같은 슬롯입니다.");
				return;
			}

			Item itemA = slotA.Inventory.GetItem(slotA.Index);
			Item itemB = slotB.Inventory.GetItem(slotB.Index);

			// 1. 셀 수 있는 아이템이고, 동일한 아이템일 경우
			//    indexA -> indexB로 개수 합치기
			if (itemA != null && itemB != null &&
				itemA.Data == itemB.Data &&
				itemA.Data.IsCountable && itemB.Data.IsCountable)
			{
				int maxAmount = itemB.MaxAmount;
				int sum = itemA.Amount + itemB.Amount;

				if (sum <= maxAmount)
				{
					itemA.SetAmount(sum);
					itemB.SetAmount(0);
				}
				else
				{
					itemA.SetAmount(maxAmount);
					itemB.SetAmount(sum - maxAmount);
				}
			}
			// 2. 일반적인 경우 : 슬롯 교체
			else
			{
				slotA.Inventory.SetItem(slotA.Index, itemB);
				slotB.Inventory.SetItem(slotB.Index, itemA);
			}

			// 두 슬롯 정보 갱신
			slotA.Inventory.UpdateSlot(slotA.Index);
			slotB.Inventory.UpdateSlot(slotB.Index);
		}
	}
}