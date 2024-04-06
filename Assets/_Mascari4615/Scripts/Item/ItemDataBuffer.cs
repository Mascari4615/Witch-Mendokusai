using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(ItemDataBuffer), menuName = "DataBuffer/" + nameof(ItemData))]
	public class ItemDataBuffer : DataBuffer<ItemData>
	{
		[System.NonSerialized] public Dictionary<int, int> itemCountDic = new();
		public override void Add(ItemData itemData)
		{
			if (itemCountDic.ContainsKey(itemData.ID))
			{
				itemCountDic[itemData.ID]++;
			}
			else
			{
				itemCountDic.Add(itemData.ID, 1);
				RuntimeItems.Add(itemData);
			}
		}

		public override bool Remove(ItemData itemData)
		{
			if (itemCountDic.ContainsKey(itemData.ID))
			{
				itemCountDic[itemData.ID]--;
				if (itemCountDic[itemData.ID] <= 0)
				{
					itemCountDic.Remove(itemData.ID);
					RuntimeItems.Remove(itemData);
				}
				return true;
			}
			return false;
		}

		public override void Clear()
		{
			RuntimeItems.Clear();
			itemCountDic.Clear();
		}
	}
}