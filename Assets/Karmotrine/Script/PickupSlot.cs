using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PickupSlot : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private Slot slot;

    private void Awake()
    {
        slot = GetComponent<Slot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot.SpecialThing == null)
            return;

        DragSlot.instance.SetSlot(slot);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slot.SpecialThing == null)
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
        
        if (!slot.canPlayerSetItem)
            return;
        
        if (slot.notOnlyOneItem)
        {
            // ChangeSlot
        }
        else
        {
            if (slot.SpecialThing == null)
            {
                // DragSlot.HoldingSlot의 Item에서 하나만 가져오기 (빼오기)
            }
            else
            {
                // slot.SpecialThing을 아이템 인벤토리애 넣기
                // DragSlot.HoldingSlot의 Item에서 하나만 가져오기 
            }
        }

        ChangeSlot();
    }

    private void ChangeSlot()
    {
        var tempItem = slot.SpecialThing;
        slot.SetSlot(DragSlot.instance.HoldingSlot.SpecialThing);
        DragSlot.instance.HoldingSlot.SetSlot(tempItem);
    }
}