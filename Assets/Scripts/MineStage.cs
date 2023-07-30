using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MineStage), menuName = "Stage/MineStage")]
public class MineStage : Stage
{
    public int StoneAmount => stoneAmount;
    [SerializeField] private int stoneAmount = 1000;
}
