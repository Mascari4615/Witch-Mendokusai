using System;
using System.Collections;
using System.Collections.Generic;
using Karmotrine.Script;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICombatCanvas : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private IntVariable maxExp;
    [SerializeField] private IntVariable curExp;
    [SerializeField] private IntVariable curLevel;
    [SerializeField] private Image expBar;
    private DateTime startTime;

    public void SetActive(bool active)
    {
        canvasGroup.alpha = active ? 1 : 0;
    }

    public void InitTime()
    {
        startTime = DateTime.Now;
    }

    private void Update()
    {
        timeText.text = (DateTime.Now - startTime).ToString(@"mm\:ss");
    }

    public void UpdateExp()
    {
        expBar.fillAmount = (float)curExp.RuntimeValue / maxExp.RuntimeValue;
        levelText.text = curLevel.RuntimeValue.ToString();
    }
}