using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public enum ComparisonOperator
	{
		Equal,
		NotEqual,
		GreaterThan,
		LessThan,
		GreaterThanOrEqualTo,
		LessThanOrEqualTo
	}

	public abstract class Criteria : ScriptableObject
	{
		public float Progress { get; protected set; }
		public abstract bool IsSatisfied();
		public abstract float GetProgress();
	}
}