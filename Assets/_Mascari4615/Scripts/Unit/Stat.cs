using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public enum StatType
	{
		// 체력
		HP_CUR = 0,
		HP_MAX = 1,

		// 경험치, 레벨
		EXP_CUR = 100,
		EXP_MAX = 101,
		LEVEL_CUR = 102,

		// 마나
		MANA_CUR = 200,
		MANA_MAX = 201,

		// 이동
		MOVEMENT_SPEED = 300,
		MOVEMENT_SPEED_BONUS = 301,

		// 스킬
		COOLTIME_BONUS = 400,

		// 데미지
		DAMAGE_BONUS = 500,

		// 키워드
		PLAYER_EXP_COLLIDER_SCALE = 10000,
		SATELLITE_COUNT = 100001,
	}

	public class Stat
	{
		private readonly Dictionary<StatType, int> stats = new();
		private readonly Dictionary<StatType, Action> events = new();
		
		public void Init(Stat newStats)
		{
			stats.Clear();
			foreach (var stat in newStats.stats)
				stats[stat.Key] = stat.Value;
		}

		public int this [StatType statType]
		{
			get
			{
				if (stats.ContainsKey(statType) == false)
					stats[statType] = 0;
				
				return stats[statType];
			}
			set
			{
				if (stats.ContainsKey(statType) == false)
					stats[statType] = 0;

				stats[statType] = value;
				
				if (events.ContainsKey(statType))
					events[statType]?.Invoke();
			}
		}

		public void AddListener(StatType statType, Action action)
		{
			if (events.ContainsKey(statType) == false)
				events[statType] = action;
			else
				events[statType] += action;
		}

		public void RemoveListener(StatType statType, Action action)
		{
			if (events.ContainsKey(statType) == false)
				return;

			events[statType] -= action;
		}
	}
}