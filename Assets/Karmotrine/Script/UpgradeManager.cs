using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Karmotrine;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance => instance;
    private static UpgradeManager instance;
    
    [SerializeField] private float upgradePow = 1.07f;
    [SerializeField] private float costPow = 3.14f;

    [SerializeField] private GameEvent upgradeEvent;

    public int[] goldPerClickCostDefault { get; private set; } = Enumerable.Repeat(1, 100).ToArray();
    public int[] goldPerClickDefault { get; private set; }= Enumerable.Repeat(1, 100).ToArray();
    public int[] goldPerSecCostDefault { get; private set; }= Enumerable.Repeat(1, 100).ToArray();
    public int[] goldPerSecDefault { get; private set; } = Enumerable.Repeat(1, 100).ToArray();

    public int[] goldPerClickLevel { get; private set; } = new int[100];
    public int[] goldPerSecLevel { get; private set; }= new int[100];

    public void UpgradeGoldPerClick(int index)
    {
        var gm = GoldManager.Instance;
        var cost = CalcUpgradeGoldPerClickCost(index);
        var goldByUpgrade = goldPerClickDefault[index] * (int)Mathf.Pow(upgradePow, goldPerClickLevel[index]);

        if (gm.gold.RuntimeValue < cost)
            return;

        gm.SubGold(cost);
        gm.AddGoldPerSec(goldByUpgrade);
        goldPerClickLevel[index]++;
        
        upgradeEvent.Raise();
    }
    public int CalcUpgradeGoldPerClickCost(int index)
    {
        return goldPerClickCostDefault[index] * (int)Mathf.Pow(costPow, goldPerClickLevel[index]);
    }
    public void UpgradeGoldPerSec(int index)
    {
        var gm = GoldManager.Instance;
        var cost = CalcUpgradeGoldPerSecCost(index);
        var goldByUpgrade = goldPerSecDefault[index] * (int)Mathf.Pow(upgradePow, goldPerSecLevel[index]);

        if (gm.gold.RuntimeValue < cost)
            return;

        gm.SubGold(cost);
        gm.AddGoldPerSec(goldByUpgrade);
        goldPerSecLevel[index]++;
        
        upgradeEvent.Raise();
    }

    public int CalcUpgradeGoldPerSecCost(int index)
    {
        return goldPerSecCostDefault[index] * (int)Mathf.Pow(costPow, goldPerSecLevel[index]);
    }
    
    private void Awake()
    {
        instance = this;
        upgradeEvent.Raise();
    }
}