using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    public bool isHoldingSomething => holdingSlot != null;
    public Slot HoldingSlot => holdingSlot;
    
    static public DragSlot instance;
    private Slot holdingSlot;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image specialThingImage;

    private void Awake()
    {
        instance = this;
    }

    public void SetSlot(Slot slot)
    {
        holdingSlot = slot;
        
        if (slot == null)
            return;
        
        specialThingImage.sprite = slot.SpecialThing.sprite;
        SetColor(1);
    }
    
    public void SetColor(float _alpha)
    {
        canvasGroup.alpha = _alpha;
    }
}