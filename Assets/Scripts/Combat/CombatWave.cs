using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(CombatWave), menuName = "Variable/CombatWave")]
public class CombatWave : Artifact
{
    public Enemy WaveEnemy => waveEnemy;
    [SerializeField] private Enemy waveEnemy;
}
