using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Mascari4615
{
	public interface ICriteria
	{
		bool Evaluate();
		float GetProgress();
	}

	public abstract class Criteria : ICriteria
	{
		public abstract int GetCurValue();
		public abstract int GetTargetValue();
		public abstract bool Evaluate();
		public virtual float GetProgress()
		{
			return (float)GetCurValue() / GetTargetValue();
		}
	}

	public class RuntimeCriteria : ICriteria
	{
		public Criteria Criteria { get; private set; }
		// 한 번만 달성하면 되는지
		public bool JustOnce { get; private set; }
		public bool IsCompleted { get; private set; }

		public bool Evaluate()
		{
			if (JustOnce && IsCompleted)
				return true;

			// Debug.Log(Criteria.Evaluate());
			return IsCompleted = Criteria.Evaluate();
		}

		public float GetProgress()
		{
			return Criteria.GetProgress();
		}

		// 동적인 조건 정보 (i.e. 에디터 타임에 정해지지 않은, 어떤 아이템이 필요하다)
		[JsonConstructor]
		public RuntimeCriteria(Criteria criteria, bool justOnce = false)
		{
			Criteria = criteria;
			JustOnce = justOnce;
		}

		public RuntimeCriteria(CriteriaInfo criteriaInfo)
		{
			Criteria = criteriaInfo.CriteriaSO.CreateCriteria(criteriaInfo);
			JustOnce = criteriaInfo.JustOnce;
		}
	}
}