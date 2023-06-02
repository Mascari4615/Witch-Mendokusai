using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour//, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] private ToolTip targetToolTip;
    [SerializeField] private bool isSpecialThings = true;
    private SpecialThing specialThing;

    private Sprite sprite;
    private string header;
    private string description;

    private bool isShowingThis = false;

    public void SetToolTip(SpecialThing _specialThing) =>
        specialThing = _specialThing;

    public void SetToolTip(Sprite _sprite, string _name, string _description)
    {
        sprite = _sprite;
        header = _name;
        description = _description;
    }

    /*public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSpecialThings && specialThing == null)
            return;

        if (targetToolTip != null)
        {
            if (isSpecialThings)
                targetToolTip.SetToolTip(specialThing);
            else
                targetToolTip.SetToolTip(sprite, header, description);

            targetToolTip.gameObject.SetActive(true);
        }
        else
        {
            if (isSpecialThings)
                ToolTipManager.Instance.Show(specialThing);
            else
                ToolTipManager.Instance.Show(sprite, header, description);
        }

        isShowingThis = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetToolTip != null)
            targetToolTip.gameObject.SetActive(false);

        ToolTipManager.Instance.Hide();
        isShowingThis = false;
    }

    private void OnDisable()
    {
        if (isShowingThis)
        {
            if (targetToolTip != null)
                targetToolTip.gameObject.SetActive(false);
            ToolTipManager.Instance.Hide();
        }
    }*/

    public void Click()
    {
        if (targetToolTip == null)
            return;
        
        if (specialThing == null)
            return;
        
        if (sprite == null && header == string.Empty && description == string.Empty)
            return;
        
        if (isSpecialThings)
            targetToolTip.SetToolTip(specialThing);
        else
            targetToolTip.SetToolTip(sprite, header, description);

        targetToolTip.gameObject.SetActive(true);
    }
}