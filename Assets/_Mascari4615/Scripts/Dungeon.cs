using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(Dungeon), menuName = "Variable/Dungeon")]
	public class Dungeon : Stage
	{
		public List<MonsterWave> MonsterWaves => monsterWaves;
		public int Reword => reword;
		public int Difficulty => difficulty;
		[SerializeField] private List<MonsterWave> monsterWaves;
		[SerializeField] private int reword;
		[SerializeField] private int difficulty;
	}
}