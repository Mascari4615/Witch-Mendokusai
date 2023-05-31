using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoodsAmountUI : MonoBehaviour
{
    [SerializeField] private IntVariable intVariable;
    [SerializeField] private TextMeshProUGUI text;
    private int curValue;
    
    private void LateUpdate()
    {
        curValue = (int)Mathf.SmoothStep(curValue, intVariable.RuntimeValue, .5f);
        text.text = curValue.ToString("N0");
    }
}
