using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MonsterWave), menuName = "Variable/CombatWave")]
public class MonsterWave : Artifact
{
    public Enemy Monster => monster;
    public float SpawnTime => spawnTime;
    [SerializeField] private Enemy monster;
    [SerializeField] private float spawnTime;
}
