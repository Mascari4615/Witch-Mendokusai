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
		[PropertyOrder(100)][field: SerializeField] public DungeonType Type { get; private set; }
		[PropertyOrder(101)][field: SerializeField] public int ClearValue { get; private set; }
		[PropertyOrder(102)][field: SerializeField] public int TimeByMinute { get; private set; }
		[PropertyOrder(103)][field: SerializeField] public List<DungeonStage> Stages { get; private set; }
		[PropertyOrder(104)][field: SerializeField] public List<MonsterWave> MonsterWaves { get; set; }
		[PropertyOrder(105)][field: SerializeField] public List<RewardInfo> Rewards { get; set; }
	}
}