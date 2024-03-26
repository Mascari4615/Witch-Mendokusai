using System;
using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	public class ItemCountCriteria : NumCriteria
	{
		public int ItemID { get; private set; }

		public ItemCountCriteria(ComparisonOperator comparisonOperator, int targetValue, int itemID) : base(comparisonOperator, targetValue)
		{
			ItemID = itemID;
		}

		public override int GetCurValue()
		{
			Inventory inventory = SOManager.Instance.ItemInventory;
			return inventory.GetItemAmount(ItemID);
		}
	}
}