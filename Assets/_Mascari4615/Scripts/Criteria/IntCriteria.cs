using System;
using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	public class IntCriteria : NumCriteria
	{
		public IntVariable IntVariable { get; private set; }

		public IntCriteria(ComparisonOperator comparisonOperator, int targetValue, IntVariable intVariable) : base(comparisonOperator, targetValue)
		{
			IntVariable = intVariable;
		}

		public override bool Evaluate()
		{
			return Evaluate_(IntVariable.RuntimeValue);
		}

		public override float GetProgress()
		{
			return GetProgress_(IntVariable.RuntimeValue);
		}
	}
}