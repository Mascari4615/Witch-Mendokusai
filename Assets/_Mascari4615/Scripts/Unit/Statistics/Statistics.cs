using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Statistics), menuName = nameof(Statistics))]
	public class Statistics : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField] private List<StatisticsInfo> initStatistics = new();

		[NonSerialized] private Dictionary<StatisticsType, int> statistics = new();

		public int this[StatisticsType type]
		{
			get
			{
				if (!statistics.ContainsKey(type))
					statistics[type] = 0;
					
				return statistics[type];
			}
			set
			{
				if (!statistics.ContainsKey(type))
					statistics[type] = 0;

				statistics[type] = value;
			}
		}

		public void OnAfterDeserialize()
		{
			statistics = new();

			foreach (StatisticsInfo info in initStatistics)
				statistics[info.type] = info.value;
		}

		public void OnBeforeSerialize()
		{
		}

		public void Load(Dictionary<StatisticsType, int> stats)
		{
			statistics = stats;
		}

		public Dictionary<StatisticsType, int> Save()
		{
			return statistics;
		}
	}
}