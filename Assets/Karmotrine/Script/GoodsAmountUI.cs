using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoodsAmountUI : MonoBehaviour
{
    [SerializeField] private IntVariable intVariable;
    [SerializeField] private TextMeshProUGUI text;

    public void UpdateUI()
    {
        text.text = intVariable.RuntimeValue.ToString();
    }
}
