using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public abstract class Stat<T> where T : Enum
	{
		private readonly Dictionary<T, int> stats = new();
		private readonly Dictionary<T, Action> events = new();
		
		public void Init(Stat<T> newStats)
		{
			stats.Clear();
			foreach (var (stat, value) in newStats.stats)
				stats[stat] = value;
		}

		public int this [T statType]
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

		public void AddListener(T statType, Action action)
		{
			if (events.ContainsKey(statType) == false)
				events[statType] = action;
			else
				events[statType] += action;
		}

		public void RemoveListener(T statType, Action action)
		{
			if (events.ContainsKey(statType) == false)
				return;

			events[statType] -= action;
		}
	}
}