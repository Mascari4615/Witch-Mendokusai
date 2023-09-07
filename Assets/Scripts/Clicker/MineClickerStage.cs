using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MineClickerStage), menuName = "Stage/MineStage")]
public class MineClickerStage : ClickerStage
{
    public int StoneAmount => stoneAmount;
    [SerializeField] private int stoneAmount = 1000;
}
