using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Mascari4615
{
	public abstract class Criteria : ICriteria
	{
		public abstract int GetCurValue();
		public abstract int GetTargetValue();
		public abstract bool Evaluate();
		public virtual float GetProgress()
		{
			return (float)GetCurValue() / GetTargetValue();
		}

		public static Criteria CreateCriteria(CriteriaInfo criteriaInfo)
		{
			return criteriaInfo.Type switch
			{
				CriteriaType.ItemCount => new ItemCountCriteria(criteriaInfo),
				CriteriaType.Stat => new StatCriteria(criteriaInfo),
				CriteriaType.Statistics => new StatisticsCriteria(criteriaInfo),
				_ => throw new ArgumentOutOfRangeException(),
			};
		}
	}
}