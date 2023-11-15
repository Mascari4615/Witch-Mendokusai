using System;
using System.Collections.Generic;
using System.Linq;
using Karmotrine.Script;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Inventory", menuName = "GameSystem/RunTimeSet/Inventory")]
public class Inventory : ScriptableObject
{
    [SerializeField] private ItemVariable lastEquippedItem;
    [SerializeField] private GameEvent onItemEquip;
    [SerializeField] private GameEvent onItemRemove;

    [field: System.NonSerialized] public Item[] Items { get; private set; }
    [System.NonSerialized] private readonly List<UIItemInventory> _inventoryUIs = new List<UIItemInventory>();

    public void RegisterInventoryUI(UIItemInventory uiItemInventory)
    {
        try
        {
            Debug.Log(_inventoryUIs[0].gameObject);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _inventoryUIs.Clear();
            // throw;
        }
        finally
        {
            _inventoryUIs.Add(uiItemInventory);
        }
    }

    public int Capacity { get; private set; }

    private int FindEmptySlotIndex(int startIndex = 0)
    {
        for (var i = startIndex; i < Capacity; i++)
        {
            if (Items[i] == null)
                return i;
        }

        return -1;
    }

    public int FindItemSlotIndex(ItemData target, int startIndex = 0)
    {
        for (var i = startIndex; i < Capacity; i++)
        {
            var current = Items[i];
            if (current == null)
                continue;

            // 아이템 종류 일치, 개수 여유 확인
            if (current.Data != target)
                continue;

            if (!current.IsMax)
                return i;
        }

        return -1;
    }

    public int FindEquipmentByGuid(Guid? guid)
    {
        if (guid == null)
            return -1;
        
        for (var i = 0; i < Capacity; i++)
        {
            var current = Items[i];
            if (current == null)
                continue;
            
            if (current.Guid == guid)
                return i;
        }

        return -1;
    }

    /// <summary> 인벤토리에 아이템 추가
    /// <para/> 넣는 데 실패한 잉여 아이템 개수 리턴
    /// <para/> 리턴이 0이면 넣는데 모두 성공했다는 의미
    /// </summary>
    public int Add(ItemData itemData, int amount = 1)
    {
        int index;

        // 1. 수량이 있는 아이템
        // if (itemData is CountableItemData ciData)
        if (itemData.IsCountable)
        {
            var findNextCountable = true;
            index = -1;

            while (amount > 0)
            {
                // 1-1. 이미 해당 아이템이 인벤토리 내에 존재하고, 개수 여유 있는지 검사
                if (findNextCountable)
                {
                    // index = FindItemSlotIndex(ciData, index + 1);
                    index = FindItemSlotIndex(itemData, index + 1);

                    // 개수 여유있는 기존재 슬롯이 더이상 없다고 판단될 경우, 빈 슬롯부터 탐색 시작
                    if (index == -1)
                    {
                        findNextCountable = false;
                    }
                    // 기존재 슬롯을 찾은 경우, 양 증가시키고 초과량 존재 시 amount에 초기화
                    else
                    {
                        // CountableItem ci = Items[index] as CountableItem;
                        // amount = ci.AddAmountAndGetExcess(amount);

                        amount = Items[index].AddAmountAndGetExcess(amount);

                        UpdateSlot(index);
                    }
                }
                // 1-2. 빈 슬롯 탐색
                else
                {
                    index = FindEmptySlotIndex(index + 1);

                    // 빈 슬롯조차 없는 경우 종료
                    if (index == -1)
                    {
                        break;
                    }
                    // 빈 슬롯 발견 시, 슬롯에 아이템 추가 및 잉여량 계산
                    else
                    {
                        // 새로운 아이템 생성
                        // CountableItem ci = ciData.CreateItem() as CountableItem;
                        // ci.SetAmount(amount);
                        var i = itemData.CreateItem();
                        i.SetAmount(amount);

                        // 슬롯에 추가
                        // Items[index] = ci;
                        Items[index] = i;

                        // 남은 개수 계산
                        // amount = (amount > ciData.MaxAmount) ? (amount - ciData.MaxAmount) : 0;
                        amount = (amount > itemData.MaxAmount) ? (amount - itemData.MaxAmount) : 0;

                        UpdateSlot(index);
                    }
                }
            }
        }
        // 2. 수량이 없는 아이템
        else
        {
            // 2-1. 1개만 넣는 경우, 간단히 수행
            if (amount == 1)
            {
                index = FindEmptySlotIndex();
                if (index != -1)
                {
                    // 아이템을 생성하여 슬롯에 추가
                    Items[index] = itemData.CreateItem();
                    amount = 0;

                    UpdateSlot(index);
                }
            }

            // 2-2. 2개 이상의 수량 없는 아이템을 동시에 추가하는 경우
            index = -1;
            for (; amount > 0; amount--)
            {
                // 아이템 넣은 인덱스의 다음 인덱스부터 슬롯 탐색
                index = FindEmptySlotIndex(index + 1);

                // 다 넣지 못한 경우 루프 종료
                if (index == -1)
                {
                    break;
                }

                // 아이템을 생성하여 슬롯에 추가
                Items[index] = itemData.CreateItem();

                UpdateSlot(index);
            }
        }

        lastEquippedItem.RuntimeValue = itemData;
        onItemEquip.Raise();

        return amount;
    }

    public void InitItems(List<InventorySlotData> savedItems)
    {
        Capacity = 30;
        var temp = new Item[Capacity];
        foreach (var itemData in savedItems)
            temp[itemData.slotIndex] = new Item(itemData.Guid, DataManager.Instance.ItemDic[itemData.itemID], itemData.itemAmount);
        Items = temp;
    }

    public List<InventorySlotData> GetInventoryData()
    {
        List<InventorySlotData> temp = new(Capacity);
        for (var i = 0; i < Items.Length; i++)
        {
            if (Items[i] == null)
                continue;

            temp.Add(new InventorySlotData(i, Items[i]));
        }

        return temp;
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < Capacity;
    }

    public ItemData GetItemData(int index)
    {
        if (!IsValidIndex(index)) return null;
        return Items[index] == null ? null : Items[index].Data;
    }

    public Item GetItem(int index)
    {
        if (!IsValidIndex(index)) return null;
        return Items[index] ?? null;
    }

    public void SetItem(int index, Item item)
    {
        if (!IsValidIndex(index))
            return;

        Items[index] = item;
        UpdateSlot(index);
    }

    // OnItemRemove.Raise();

    private void UpdateSlot(params int[] indices)
    {
        foreach (var i in indices)
        {
            UpdateSlot(i);
        }
    }

    public void UpdateSlot(int index)
    {
        if (!IsValidIndex(index)) return;

        if (Items[index] != null)
            if (Items[index].Data.IsCountable)
                if (Items[index].IsEmpty)
                    Items[index] = null;

        var item = Items[index];

        foreach (var inventoryUI in _inventoryUIs)
        {
            inventoryUI.UpdateSlotUI(index, item);

            // 1. 아이템이 슬롯에 존재하는 경우
            //if (item != null)
              //  inventoryUI.UpdateSlotFilterState(index, item.Data);
        }
    }
}
