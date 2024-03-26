using System;
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

	public interface ICriteria
	{
		bool Evaluate();
		float GetProgress();
	}

	public abstract class Criteria : ICriteria
	{
		public abstract bool Evaluate();
		public abstract float GetProgress();
	}
}