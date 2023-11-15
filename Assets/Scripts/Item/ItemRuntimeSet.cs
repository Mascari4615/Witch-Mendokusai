using System.Collections.Generic;
using UnityEngine;

public abstract class ItemRuntimeSet : RuntimeSet<ItemData>
{
    [System.NonSerialized] public Dictionary<int, int> itemCountDic = new();

    public override void Add(ItemData itemData)
    {
        if (!Items.Contains(itemData))
        {
            Items.Add(itemData);
            itemCountDic.Add(itemData.ID, 1);
        }
        else itemCountDic[itemData.ID] += 1;
    }

    public override void Remove(ItemData itemData)
    {
        if (Items.Contains(itemData))
        {
            if (itemCountDic[itemData.ID] > 1) itemCountDic[itemData.ID] -= 1;
            else if (itemCountDic[itemData.ID] == 1)
            {
                itemCountDic.Remove(itemData.ID);
                Items.Remove(itemData);
            }
            else Debug.LogWarning($"RunTimeSet<Item> : Item.count�� ������ �� �� ����, {itemData.Name}");
        }
        else Debug.LogWarning($"RunTimeSet<Item> : �������� �ʴ� ������ ���� �õ�, {itemData.Name}");
    }

    public override void Clear()
    {
        if (Items == null) return;

        int itemsCount = Items.Count;
        for (int i = 0; i < itemsCount; i++)
        {
            if (itemCountDic != null)
            {
                int itemCount = itemCountDic[Items[0].ID];
                for (int j = 0; j < itemCount; j++)
                {
                    Remove(Items[0]);
                }
            }
            else
            {
                Debug.LogError("������ ���� ��ųʸ��� �������� �ʽ��ϴ�.");
            }
        }

        Items.Clear();
        itemCountDic.Clear();
    }
}