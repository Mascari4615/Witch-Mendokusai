using System;
using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(IntCriteriaSO), menuName = "Criteria/" + nameof(IntCriteriaSO))]
	public class IntCriteriaSO : NumCriteriaSO
	{
		[SerializeField] private IntVariable intVariable;

		public override Criteria CreateCriteria()
		{
			return new IntCriteria(comparisonOperator, targetValue, intVariable);
		}
	}
}