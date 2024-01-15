using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = "ItemDataBuffer", menuName = "GameSystem/DataBuffer/Item")]
	public class ItemDataBuffer : DataBuffer<ItemData>
	{
		[System.NonSerialized] public Dictionary<int, int> itemCountDic = new();
		public override void AddItem(ItemData itemData)
		{
			if (!RuntimeItems.Contains(itemData))
			{
				RuntimeItems.Add(itemData);
				itemCountDic.Add(itemData.ID, 1);
			}
			else itemCountDic[itemData.ID] += 1;
		}

		public override void RemoveItem(ItemData itemData)
		{
			if (RuntimeItems.Contains(itemData))
			{
				if (itemCountDic[itemData.ID] > 1) itemCountDic[itemData.ID] -= 1;
				else if (itemCountDic[itemData.ID] == 1)
				{
					itemCountDic.Remove(itemData.ID);
					RuntimeItems.Remove(itemData);
				}
				else Debug.LogWarning($"RunTimeSet<Item> : Item.count�� ������ �� �� ����, {itemData.Name}");
			}
			else Debug.LogWarning($"RunTimeSet<Item> : �������� �ʴ� ������ ���� �õ�, {itemData.Name}");
		}

		public override void ClearBuffer()
		{
			if (RuntimeItems == null) return;

			int itemsCount = RuntimeItems.Count;
			for (int i = 0; i < itemsCount; i++)
			{
				if (itemCountDic != null)
				{
					int itemCount = itemCountDic[RuntimeItems[0].ID];
					for (int j = 0; j < itemCount; j++)
					{
						RemoveItem(RuntimeItems[0]);
					}
				}
				else
				{
					Debug.LogError("������ ���� ��ųʸ��� �������� �ʽ��ϴ�.");
				}
			}

			RuntimeItems.Clear();
			itemCountDic.Clear();
		}
	}
}