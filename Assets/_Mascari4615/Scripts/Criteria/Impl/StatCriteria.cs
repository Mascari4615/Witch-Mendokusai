using System;
using System.Collections;
using System.Collections.Generic;
using Mascari4615;
using UnityEngine;

namespace Mascari4615
{
	public class StatCriteria : NumCriteria
	{
		public UnitStatType Type { get; private set; }

		public StatCriteria(CriteriaInfo criteriaInfo) : base(criteriaInfo)
		{
			Type = (criteriaInfo.Data as UnitStatData).Type;
		}

		private UnitStat PlayerStat => Player.Instance.UnitStat;

		public override int GetCurValue()
		{
			return PlayerStat[Type];
		}
	}
}