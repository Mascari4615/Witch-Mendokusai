using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	public abstract class NumCriteriaSO : CriteriaSO
	{
		[SerializeField] protected ComparisonOperator comparisonOperator;
		[SerializeField] protected int targetValue;
	}
}