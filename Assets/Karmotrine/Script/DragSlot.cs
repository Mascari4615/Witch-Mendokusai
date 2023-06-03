using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    public bool isHoldingSomething => _holdingSlot != null;
    public ItemSlot HoldingSlot => _holdingSlot;
    
    static public DragSlot instance;
    private ItemSlot _holdingSlot;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image specialThingImage;

    private void Awake()
    {
        instance = this;
    }

    public void SetSlot(ItemSlot slot)
    {
        _holdingSlot = slot;
        
        if (slot == null)
            return;
        
        specialThingImage.sprite = slot.Sprite;
        SetColor(1);
    }
    
    public void SetColor(float _alpha)
    {
        canvasGroup.alpha = _alpha;
    }
}