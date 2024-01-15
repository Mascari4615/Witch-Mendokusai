using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(IntCriteria), menuName = "Criteria/Int")]
	public class IntCriteria : Criteria
	{
		[SerializeField] private IntVariable intVariable;
		[SerializeField] private int target;

		public override bool HasComplete()
		{
			return intVariable.RuntimeValue >= target;
		}
	}
}