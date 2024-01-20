using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(MonsterWave), menuName = "Variable/MonsterWave")]
	public class MonsterWave : Artifact
	{
		[field: Header("_" + nameof(MonsterWave))]
		[field: SerializeField] public Monster[] Monsters { get; set; }
		[field: SerializeField] public float SpawnTime { get; set; }
	}
}