using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI hpBarText;
    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private bool disableOnDied = false;

    private Enemy curEnemy;

    public void UpdateEnemy(Enemy targetEnemy, int curHp)
    {
        curEnemy = targetEnemy;
        nameText.text = curEnemy.Name;

        UpdateUI(curHp);
    }

    public void UpdateUI(int curHp)
    {
        _canvasGroup.alpha = 1;
        hpBarText.text = $"{curHp} / {curEnemy.maxHp}";
        hpBar.fillAmount = (float)curHp / curEnemy.maxHp;

        if (curHp == 0)
        {
            if (disableOnDied)
                _canvasGroup.alpha = 0;
        }
    }

    public void Disable()
    {
        _canvasGroup.alpha = 0;
    }
}