using System.Collections;
using System.Collections.Generic;
using Karmotrine;
using UnityEngine;

public class ClickButton : MonoBehaviour
{
    public void Click()
    {
        GoldManager.Instance.AddGold(GoldManager.Instance.goldPerClick.RuntimeValue);
    }
}
