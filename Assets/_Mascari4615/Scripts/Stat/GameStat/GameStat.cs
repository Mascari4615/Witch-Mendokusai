using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class GameStat : Stat<GameStatType>
	{
		public void UpdateData()
		{
			stats[GameStatType.TOTAL_MONSTER_KILL] += stats[GameStatType.MONSTER_KILL];
			stats[GameStatType.TOTAL_MONSTER_KILL] += stats[GameStatType.BOSS_KILL];
			stats[GameStatType.TOTAL_DUNGEON_TIME] += stats[GameStatType.DUNGEON_TIME];

			stats[GameStatType.MONSTER_KILL] = 0;
			stats[GameStatType.BOSS_KILL] = 0;
			stats[GameStatType.DUNGEON_TIME] = 0;
		}

		public GameStat()
		{
			InitAllZero();
		}

		public void Load(Dictionary<GameStatType, int> saveGameStat)
		{
			InitAllZero();

			// 저장된 통계 불러오기
			foreach (var (key, value) in saveGameStat)
				stats[key] = value;
		}

		public Dictionary<GameStatType, int> Save()
		{
			return stats;
		}
	}
}