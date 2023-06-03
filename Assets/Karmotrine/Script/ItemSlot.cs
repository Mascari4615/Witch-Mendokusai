using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : Slot, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private Inventory _inventory;
    public Inventory Inventory => _inventory;
    
    public bool onlyOneItem = false;
    public bool canPlayerSetItem = true;

    public void SetInventory(Inventory inventory) => _inventory = inventory;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!HasItem)
            return;

        DragSlot.instance.SetSlot(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!HasItem)
            return;

        DragSlot.instance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0);
        DragSlot.instance.SetSlot(null);
    }

    // DragSlot이 위에 떨어졌을 때
    public void OnDrop(PointerEventData eventData)
    {
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
            if (!HasItem)
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

        var slotA = this;
        var slotB = DragSlot.instance.HoldingSlot;
        
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
                itemA.SetAmount(0);
                itemB.SetAmount(sum);
            }
            else
            {
                itemA.SetAmount(sum - maxAmount);
                itemB.SetAmount(maxAmount);
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