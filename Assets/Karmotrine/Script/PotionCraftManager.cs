using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class PotionCraftManager : MonoBehaviour
{
    [SerializeField] private ItemSlot[] slots;
    [SerializeField] private ItemSlot resultSlot;
    [SerializeField] private Inventory craftTableInventory;

    private void Awake()
    {
        craftTableInventory.InitItems(new List<InventorySlotData>());
    }

    public void TryCraft()
    {
        Debug.Log("TryCraft");

        var recipeToList = new List<int>();

        foreach (var slot in slots)
        {
            if (!slot.HasItem)
                continue;

            recipeToList.Add(craftTableInventory.GetItem(slot.Index).Data.ID);
        }

        recipeToList.Sort();
        var key = String.Join(',', recipeToList);
        if (!DataManager.Instance.craftDic.ContainsKey(key))
            return;
        var newItem = new Item(DataManager.Instance.ItemDic[DataManager.Instance.craftDic[key]]);
        craftTableInventory.SetItem(0, newItem);
        
        Debug.Log(newItem);
    }
}