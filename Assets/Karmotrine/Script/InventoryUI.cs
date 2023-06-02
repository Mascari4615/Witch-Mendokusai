using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [HideInInspector] public List<Slot> slots = new();

    private void Awake()
    {
        slots = GetComponentsInChildren<Slot>().ToList();
    }

    private void OnEnable() => UpdateUI();

    public void UpdateUI()
    {
        for (var i = 0; i < slots.Count; i++)
        {
            if (i < inventory.Capacity)
            {
                slots[i].SetSlot(inventory.Items[i]?.Data);
                slots[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].SetSlot(null);
                slots[i].gameObject.SetActive(false);
            }
        }
    }
}
