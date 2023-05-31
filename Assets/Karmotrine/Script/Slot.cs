using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    public ToolTipTrigger toolTipTrigger;
    [SerializeField] protected Image image;
    public SpecialThing SpecialThing { get; private set; }
    protected int count = 1;
    [SerializeField] protected TextMeshProUGUI countTextField;
    [SerializeField] protected TextMeshProUGUI priceText;
    [SerializeField] protected TextMeshProUGUI nameText;
    [SerializeField] protected TextMeshProUGUI descriptionText;

    public virtual void SetSlot(SpecialThing specialThing, int count = 1)
    {
        SpecialThing = specialThing;
        this.count = count;

        UpdateUI();
    }

    protected virtual void UpdateUI()
    {
        image.sprite = SpecialThing?.sprite;

        if (toolTipTrigger != null)
            toolTipTrigger.SetToolTip(SpecialThing);

        // if (countTextField != null) countTextField.text = DataManager.Instance.wgItemInven.itemCountDic[(SpecialThing as Item).ID].ToString();
        if (countTextField != null)
            countTextField.text = count.ToString();

        if (nameText != null)
            nameText.text = SpecialThing?.name;

        // if (priceText != null)
        //	  priceText.text = (SpecialThing as HasPrice)?.price.ToString();

        if (descriptionText != null)
            descriptionText.text = SpecialThing?.description;
    }
}