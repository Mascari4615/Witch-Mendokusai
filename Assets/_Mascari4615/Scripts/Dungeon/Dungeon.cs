using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Dungeon), menuName = "Variable/Dungeon")]
	public class Dungeon : DataSO
	{
		[field: Header("_" + nameof(Dungeon))]
		[field: SerializeField] public DungeonType Type { get; private set; }
		[field: SerializeField] public int TimeByMinute { get; private set; }
		[field: SerializeField] public List<DungeonStage> Stages { get; private set; }
		[field: SerializeField] public List<MonsterWave> MonsterWaves { get; set; }
		[field: SerializeField] public List<RewardInfo> Rewards { get; set; }
	}
}