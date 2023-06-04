using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GoldUI : MonoBehaviour
{
    [FormerlySerializedAs("intVariable")] [SerializeField] private IntVariable curGold;
    [SerializeField] private TextMeshProUGUI text;
    private int curValue;
    
    private void LateUpdate()
    {
        curValue = (int)Mathf.Ceil(Mathf.SmoothStep(curValue, curGold.RuntimeValue, .5f));
        text.text = curValue.ToString("N0") + "냥";
    }
}
