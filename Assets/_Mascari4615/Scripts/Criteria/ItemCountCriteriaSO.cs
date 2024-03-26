using System;
using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(ItemCountCriteriaSO), menuName = "Criteria/" + nameof(ItemCountCriteriaSO))]
	public class ItemCountCriteriaSO : NumCriteriaSO
	{
		[SerializeField] private ItemData itemData;

		public override void OnAfterDeserialize()
		{
			Data = new ItemCountCriteria(comparisonOperator, targetValue, itemData.ID);
		}
	}
}