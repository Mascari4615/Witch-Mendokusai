using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class EquipTooltip : MonoBehaviour
{
    private readonly WaitForSeconds ws1 = new (1f);
    private readonly Queue<ItemData> toolTipStacks = new();
    [SerializeField] private GameObject toolTip;
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private Image image;
    [SerializeField] private ItemVariable lastEquippedItem;

    private void Awake()
    {
        toolTip.SetActive(false);
    }

    public void EquipItem()
    {
        toolTipStacks.Enqueue(lastEquippedItem.RuntimeValue);
        if (!toolTip.activeSelf) StartCoroutine(ShowToolTip());
    }

    private IEnumerator ShowToolTip()
    {
        toolTip.SetActive(true);

        while (toolTipStacks.Count > 0)
        {
            var itemData = toolTipStacks.Dequeue();
            nameField.text = itemData.Name;
            image.sprite = itemData.sprite;
            yield return ws1;
        }
        
        toolTip.SetActive(false);
    }

    public void StopToolTip()
    {
        toolTip.SetActive(false);
        StopAllCoroutines();
    }
}