using UnityEngine;
[CreateAssetMenu(fileName = nameof(Enemy), menuName = "Variable/Enemy")]
public class Enemy : SpecialThing
{
    public int hp;
    public SpecialThingWithPercentage[] Loots;
}