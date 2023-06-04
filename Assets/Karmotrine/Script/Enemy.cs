using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(Enemy), menuName = "Variable/Enemy")]
public class Enemy : Artifact
{
    public int MaxHp => maxHp;
    public ArtifactWithPercentage[] Loots => loots;

    [SerializeField] private int maxHp;
    [SerializeField] private ArtifactWithPercentage[] loots;
}