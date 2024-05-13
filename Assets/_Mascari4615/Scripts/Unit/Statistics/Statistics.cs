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

		public void UpdateData()
		{
			statistics[StatisticsType.TOTAL_MONSTER_KILL] += statistics[StatisticsType.MONSTER_KILL];
			statistics[StatisticsType.TOTAL_MONSTER_KILL] += statistics[StatisticsType.BOSS_KILL];
			statistics[StatisticsType.TOTAL_DUNGEON_TIME] += statistics[StatisticsType.DUNGEON_TIME];

			statistics[StatisticsType.MONSTER_KILL] = 0;
			statistics[StatisticsType.BOSS_KILL] = 0;
			statistics[StatisticsType.DUNGEON_TIME] = 0;
		}

		public void OnAfterDeserialize()
		{
			statistics = new();

			// 모든 통계 초기화
			foreach (StatisticsType type in Enum.GetValues(typeof(StatisticsType)))
				statistics[type] = 0;

			// 초기 통계
			foreach (StatisticsInfo info in initStatistics)
				statistics[info.type] = info.value;
		}

		public void OnBeforeSerialize()
		{
		}

		public void Load(Dictionary<StatisticsType, int> saveStatistics)
		{
			// 저장된 통계 불러오기
			foreach (var (key, value) in saveStatistics)
				statistics[key] = value;
		}

		public Dictionary<StatisticsType, int> Save()
		{
			return statistics;
		}
	}
}