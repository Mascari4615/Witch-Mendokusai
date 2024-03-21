using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(ItemCountCriteria), menuName = "Criteria/" + nameof(ItemCountCriteria))]
	public class ItemCountCriteria : NumCriteria
	{
		[SerializeField] private ItemData itemData;

		private int GetCount()
		{
			Inventory inventory = SOManager.Instance.ItemInventory;
			return inventory.GetItemAmount(itemData.ID);
		}

		public override bool IsSatisfied()
		{
			return IsSatisfied_(GetCount());
		}
		
		public override float GetProgress()
		{
			return GetProgress_(GetCount());
		}
	}
}