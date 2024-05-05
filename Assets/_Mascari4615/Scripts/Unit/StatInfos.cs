using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[Serializable]
	public struct StatInfo
	{
		public StatType stat;
		public int value;
	}

	[Serializable]
	public class StatInfos
	{
		[SerializeField] private List<StatInfo> initStats = new()
		{
			new() { stat = StatType.HP_MAX, value = 20 },
			new() { stat = StatType.MOVEMENT_SPEED, value = 3 },
		};

		public Stat GetStat()
		{
			Stat Stat = new();
			foreach (StatInfo statInfo in initStats)
				Stat[statInfo.stat] = statInfo.value;

			return Stat;
		}
	}
}