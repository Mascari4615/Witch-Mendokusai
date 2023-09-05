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

    [SerializeField] protected Artifact defaultArtifact;
    [SerializeField] protected Image image;
    [SerializeField] protected TextMeshProUGUI nameText;
    [SerializeField] protected TextMeshProUGUI countTextField;
    [SerializeField] protected TextMeshProUGUI descriptionText;

    private void Awake()
    {
        if (defaultArtifact != null)
            UpdateUI(defaultArtifact);
    }

    public void SetSlotIndex(int index) => Index = index;
    public virtual void UpdateUI(Artifact artifact, int amount = 1)
    {
        toolTipTrigger?.SetToolTip(artifact);

        image.sprite = artifact?.Thumbnail;
        image.color = artifact != null ? Color.white : Color.white * 0;

        if (nameText != null)
            nameText.text = artifact?.Name;
        
        if (countTextField != null)
            countTextField.text = HasItem ? amount.ToString() : string.Empty;

        if (descriptionText != null)
            descriptionText.text = artifact?.Description;
    }
}