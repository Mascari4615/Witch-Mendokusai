using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(Enemy), menuName = "Variable/Enemy")]
public class Enemy : SpecialThing
{
    [FormerlySerializedAs("hp")] public int maxHp;
    public SpecialThingWithPercentage[] Loots;
}