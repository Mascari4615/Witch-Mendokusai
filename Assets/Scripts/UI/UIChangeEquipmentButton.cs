using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChangeEquipmentButton : MonoBehaviour
{
    [SerializeField] private UIDollStatePanel uiDollStatePanel;
    [SerializeField] private Slot slot;

    public void ChangeEquipment()
    {
        uiDollStatePanel.ApplyNewArtifact(slot.Index);
    }
}
