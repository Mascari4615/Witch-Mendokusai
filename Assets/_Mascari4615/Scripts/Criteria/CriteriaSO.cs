using System;
using UnityEngine;

namespace Mascari4615
{
	public abstract class CriteriaSO : ScriptableObject
	{
		public abstract Criteria CreateCriteria(CriteriaInfo criteriaInfo);
	}
}