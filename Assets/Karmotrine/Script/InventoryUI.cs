using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class InventoryUI<T> : MonoBehaviour
{
    [SerializeField] private RunTimeSet<T> Inventory;
    [HideInInspector] public List<Slot> slots = new();

    private void Awake()
    {
        slots = GetComponentsInChildren<Slot>().ToList();
    }

    private void OnEnable() => Initialize();

    public void Initialize()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < Inventory.Items.Count)
            {
                slots[i].SetSlot(Inventory.Items[i] as SpecialThing);
                slots[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }
}