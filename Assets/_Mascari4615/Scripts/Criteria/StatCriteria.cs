using System;
using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	public class StatCriteria : NumCriteria
	{
		public StatType Type { get; private set; }

		public StatCriteria(CriteriaInfo criteriaInfo, StatType type) : base(criteriaInfo)
		{
			Type = type;
		}

		private Stat PlayerStat => Player.Instance.Stat;

		public override int GetCurValue()
		{
			return PlayerStat[Type];
		}
	}
}