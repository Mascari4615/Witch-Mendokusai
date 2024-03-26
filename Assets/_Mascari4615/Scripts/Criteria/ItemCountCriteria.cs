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

		private int GetCount()
		{
			Inventory inventory = SOManager.Instance.ItemInventory;
			return inventory.GetItemAmount(ItemID);
		}

		public override bool Evaluate()
		{
			return Evaluate_(GetCount());
		}
		
		public override float GetProgress()
		{
			return GetProgress_(GetCount());
		}
	}
}