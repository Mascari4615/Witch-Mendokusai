using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    public int Index { get; private set; }
    public Sprite Sprite => image.sprite;
    public bool HasItem => image.sprite != null;

    public ToolTipTrigger toolTipTrigger;

    [SerializeField] protected Image image;
    [SerializeField] protected TextMeshProUGUI nameText;
    [SerializeField] protected TextMeshProUGUI countTextField;
    [SerializeField] protected TextMeshProUGUI descriptionText;

    public void SetSlotIndex(int index) => Index = index;
    public virtual void UpdateUI(SpecialThing specialThing, int amount = 1)
    {
        toolTipTrigger?.SetToolTip(specialThing);

        image.sprite = specialThing?.sprite;

        if (nameText != null)
            nameText.text = specialThing?.Name;
        
        if (countTextField != null)
            countTextField.text = HasItem ? amount.ToString() : string.Empty;

        if (descriptionText != null)
            descriptionText.text = specialThing?.description;
    }
}