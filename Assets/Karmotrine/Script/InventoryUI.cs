using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [HideInInspector] public List<ItemSlot> slots = new();
    [SerializeField] private Transform slotsParent;

    private void Awake()
    {
        slots = slotsParent.GetComponentsInChildren<ItemSlot>().ToList();
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].SetSlotIndex(i);
            slots[i].SetInventory(inventory);
        }

        inventory.RegisterInventoryUI(this);
    }

    // private void OnEnable() => UpdateUI();

    public void UpdateUI()
    {
        for (var i = 0; i < slots.Count; i++)
        {
            if (i < inventory.Capacity)
            {
                slots[i].UpdateUI(inventory._items[i]?.Data, inventory._items[i] != null ? inventory._items[i].Amount : 1);
                slots[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].UpdateUI(null);
                slots[i].gameObject.SetActive(false);
            }
        }
    }
    
    public void UpdateSlotUI(int index, Item item)
    {
        slots[index].UpdateUI(item?.Data, item?.Amount ?? 1);
        UpdateUI();
    }
    
    public void UpdateSlotFilterState(int index, ItemData itemData)
    {
        /*bool isFiltered = true;

        // null인 슬롯은 타입 검사 없이 필터 활성화
        if(itemData != null)
            switch (_currentFilterOption)
            {
                case FilterOption.Equipment:
                    isFiltered = (itemData is EquipmentItemData);
                    break;

                case FilterOption.Portion:
                    isFiltered = (itemData is PortionItemData);
                    break;
            }

        slots[index].SetItemAccessibleState(isFiltered);*/
    }
}
