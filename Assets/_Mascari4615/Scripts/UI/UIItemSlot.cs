
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

	public class UIItemSlot : UISlot, IPointerClickHandler
	{
		protected TextMeshProUGUI priceText;

		public UIItemGrid UIItemGrid { get; private set; }
		public Inventory Inventory => UIItemGrid.DataBufferSO as Inventory;

		public bool onlyOneItem = false;
		public bool canPlayerSetItem = true;
		public bool canDrag = true;
		private PriceType priceType = PriceType.Buy;

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

			if (DataSO)
			{
				ItemData itemData = DataSO as ItemData;
				priceText.text = (priceType == PriceType.Buy) ? itemData.PurchasePrice.ToString() : itemData.SalePrice.ToString();
			}
			else
			{
				priceText.text = string.Empty;
			}
		}

		public void SetUIItemGrid(UIItemGrid itemGridUI) => UIItemGrid = itemGridUI;
		public void SetPriceType(PriceType priceType)
		{
			this.priceType = priceType;
			UpdateUI();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			switch (eventData.button)
			{
				case PointerEventData.InputButton.Left:
					Debug.Log("Left Click");
					if (UIHoldingSlot.Instance.IsHolding)
					{
						if (Inventory.GetItem(Index) == null)
						{
							// Debug.Log("L:A");
							// 들고 있는 아이템을 슬롯에 놓기
							Item item = UIHoldingSlot.Instance.DropSlot();
							Inventory.SetItem(Index, item);
						}
						else
						{
							// Debug.Log("L:B");
							// 이미 들고 있는 아이템이 있으면 들고 있는 아이템과 슬롯을 교체
							UIHoldingSlot.Instance.SwapSlot(this);
						}
					}
					else
					{
						// Debug.Log("L:C");
						// 집기
						UIHoldingSlot.Instance.HoldSlot(this);
					}
					break;
				case PointerEventData.InputButton.Right:
					Debug.Log("Right Click");
					// 절반 집기
					// 들고 있는 아이템이 있으면 들고 있는 아이템 하나 놓기
					// 들고 있는 아이템이 있는데 아이템이 있는 슬롯에 우클릭한거면 슬롯 교체
					break;
				case PointerEventData.InputButton.Middle:
					Debug.Log("Middle Click");
					break;
			}
		}
	}
}