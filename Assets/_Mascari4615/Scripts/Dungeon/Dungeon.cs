using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Dungeon), menuName = "Variable/Dungeon")]
	public class Dungeon : Stage
	{
		[field: Header("_" + nameof(Dungeon))]
		[field: SerializeField] public List<MonsterWave> MonsterWaves { get; set; }
		[field: SerializeField] public List<RewardInfo> Rewards { get; set; }
		[field: SerializeField] public int Difficulty { get; set; }
	}
}