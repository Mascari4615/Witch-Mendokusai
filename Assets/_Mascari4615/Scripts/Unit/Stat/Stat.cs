using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
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