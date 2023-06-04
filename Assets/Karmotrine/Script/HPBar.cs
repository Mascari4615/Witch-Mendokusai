using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI hpBarText;
    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private bool disableOnDied = false;

    private Enemy _curEnemy;

    public void UpdateEnemy(Enemy targetEnemy, int curHp)
    {
        _curEnemy = targetEnemy;
        nameText.text = _curEnemy.Name;

        UpdateUI(curHp);
    }

    public void UpdateUI(int curHp)
    {
        canvasGroup.alpha = 1;
        hpBarText.text = $"{curHp} / {_curEnemy.MaxHp}";
        hpBar.fillAmount = (float)curHp / _curEnemy.MaxHp;

        if (curHp != 0)
            return;
        
        if (disableOnDied)
            canvasGroup.alpha = 0;
    }

    public void Disable()
    {
        canvasGroup.alpha = 0;
    }
}