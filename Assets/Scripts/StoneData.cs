using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(StoneData), menuName = "Variable/StoneData")]
public class StoneData : Enemy
{
    public int Life => life;
    [SerializeField] private int life;
}
