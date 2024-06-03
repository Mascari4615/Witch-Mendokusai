using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(GameStat), menuName = nameof(GameStat))]
	public class GameStat : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField] private List<GameStatInfo> initGameStat = new();

		[NonSerialized] private Dictionary<GameStatType, int> gameStat = new();

		public int this[GameStatType type]
		{
			get
			{
				if (!gameStat.ContainsKey(type))
					gameStat[type] = 0;

				return gameStat[type];
			}
			set
			{
				if (!gameStat.ContainsKey(type))
					gameStat[type] = 0;

				gameStat[type] = value;
			}
		}

		public void UpdateData()
		{
			gameStat[GameStatType.TOTAL_MONSTER_KILL] += gameStat[GameStatType.MONSTER_KILL];
			gameStat[GameStatType.TOTAL_MONSTER_KILL] += gameStat[GameStatType.BOSS_KILL];
			gameStat[GameStatType.TOTAL_DUNGEON_TIME] += gameStat[GameStatType.DUNGEON_TIME];

			gameStat[GameStatType.MONSTER_KILL] = 0;
			gameStat[GameStatType.BOSS_KILL] = 0;
			gameStat[GameStatType.DUNGEON_TIME] = 0;
		}

		public void OnAfterDeserialize()
		{
			gameStat = new();

			// 모든 통계 초기화
			foreach (GameStatType type in Enum.GetValues(typeof(GameStatType)))
				gameStat[type] = 0;

			// 초기 통계
			foreach (GameStatInfo info in initGameStat)
				gameStat[info.type] = info.value;
		}

		public void OnBeforeSerialize()
		{
		}

		public void Load(Dictionary<GameStatType, int> saveGameStat)
		{
			// 저장된 통계 불러오기
			foreach (var (key, value) in saveGameStat)
				gameStat[key] = value;
		}

		public Dictionary<GameStatType, int> Save()
		{
			return gameStat;
		}
	}
}