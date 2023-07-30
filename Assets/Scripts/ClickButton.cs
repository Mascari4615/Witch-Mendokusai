using System.Collections;
using System.Collections.Generic;
using Karmotrine;
using UnityEngine;

public class ClickButton : MonoBehaviour
{
    [SerializeField] private ClickerManager _clickerManager;
    public void Click()
    {
        GoldManager.Instance.AddGold(GoldManager.Instance.goldPerClick.RuntimeValue);
        _clickerManager.Click();
    }
}
