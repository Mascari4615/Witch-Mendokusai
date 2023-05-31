using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PickupSlot : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private Camera camera;
    private Slot slot;

    private void Awake()
    {
        slot = GetComponent<Slot>();
        camera = Camera.main;
    }
    
    // 마우스 드래그가 시작 됐을 때 발생하는 이벤트
    public void OnBeginDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetSlot(slot);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        DragSlot.instance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0);
        DragSlot.instance.SetSlot(null);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.isHoldingSomething)
            ChangeSlot();
    }

    private void ChangeSlot()
    {
        SpecialThing _tempItem = slot.SpecialThing;
        slot.SetSlot(DragSlot.instance.TargetSlot.SpecialThing);
        DragSlot.instance.TargetSlot.SetSlot(_tempItem);
    }
}