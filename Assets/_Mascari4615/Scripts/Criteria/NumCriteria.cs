using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	public abstract class NumCriteria : Criteria
	{
		[field: SerializeField] public ComparisonOperator ComparisonOperator { get; private set; }
		[field: SerializeField] public int TargetValue { get; private set; }

		protected bool IsSatisfied_(int curValue)
		{
			return ComparisonOperator switch
			{
				ComparisonOperator.Equal => curValue == TargetValue,
				ComparisonOperator.NotEqual => curValue != TargetValue,
				ComparisonOperator.GreaterThan => curValue > TargetValue,
				ComparisonOperator.LessThan => curValue < TargetValue,
				ComparisonOperator.GreaterThanOrEqualTo => curValue >= TargetValue,
				ComparisonOperator.LessThanOrEqualTo => curValue <= TargetValue,
				_ => throw new System.ArgumentOutOfRangeException(),
			};
		}

		public float GetProgress_(int curValue)
		{
			return (float)curValue / TargetValue;
		}
	}
}