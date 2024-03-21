using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(IntCriteria), menuName = "Criteria/Int")]
	public class IntCriteria : NumCriteria
	{
		[SerializeField] private IntVariable intVariable;

		public override bool IsSatisfied()
		{
			return IsSatisfied_(intVariable.RuntimeValue);
		}
		
		public override float GetProgress()
		{
			return GetProgress_(intVariable.RuntimeValue);
		}
	}
}