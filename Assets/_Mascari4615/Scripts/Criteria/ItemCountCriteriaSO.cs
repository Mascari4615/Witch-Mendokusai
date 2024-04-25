using System;
using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(ItemCountCriteriaSO), menuName = "Criteria/" + nameof(ItemCountCriteriaSO))]
	public class ItemCountCriteriaSO : CriteriaSO
	{
		[SerializeField] private ItemData itemData;
		
		public override Criteria CreateCriteria(CriteriaInfo criteriaInfo)
		{
			return new ItemCountCriteria(criteriaInfo, itemData.ID);
		}
	}
}