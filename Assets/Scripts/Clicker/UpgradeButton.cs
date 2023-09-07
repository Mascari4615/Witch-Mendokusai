using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    enum UpgradeType
    {
        GoldPerClick,
        GoldPerSec
    }
    
    [SerializeField] private UpgradeType upgradeType;
    [SerializeField] private int index;
    [SerializeField] private TextMeshProUGUI costText;

    public void Click()
    {
        switch (upgradeType)
        {
            case UpgradeType.GoldPerClick:
                UpgradeManager.Instance.UpgradeGoldPerClick(index);
                break;
            case UpgradeType.GoldPerSec:
                UpgradeManager.Instance.UpgradeGoldPerSec(index);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void UpdateUI()
    {
        costText.text = upgradeType switch
        {
            UpgradeType.GoldPerClick => UpgradeManager.Instance.CalcUpgradeGoldPerClickCost(index).ToString(),
            UpgradeType.GoldPerSec => UpgradeManager.Instance.CalcUpgradeGoldPerSecCost(index).ToString(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
