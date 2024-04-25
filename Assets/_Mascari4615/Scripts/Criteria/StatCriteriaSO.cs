using System;
using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(StatCriteriaSO), menuName = "Criteria/" + nameof(StatCriteriaSO))]
	public class StatCriteriaSO : CriteriaSO
	{
		[SerializeField] private StatType statType;

		public override Criteria CreateCriteria(CriteriaInfo criteriaInfo)
		{
			return new StatCriteria(criteriaInfo, statType);
		}
	}
}