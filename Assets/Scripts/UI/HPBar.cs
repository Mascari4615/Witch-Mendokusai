using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mascari4615
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] private Image hpBar;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI hpBarText;
        [SerializeField] private TextMeshProUGUI nameText;

        [SerializeField] private bool disableOnDied = false;

        [SerializeField] private EnemyObjectVariable lastHitEnemyObject;

        private EnemyObject _curEnemyObject;

        private void OnEnable()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            canvasGroup.alpha = 1;

            _curEnemyObject = lastHitEnemyObject.RuntimeValue;
            nameText.text = _curEnemyObject.UnitData.Name;

            hpBarText.text = $"{_curEnemyObject.CurHp} / {_curEnemyObject.MaxHp}";
            hpBar.fillAmount = (float)_curEnemyObject.CurHp / _curEnemyObject.MaxHp;

            if (_curEnemyObject.CurHp != 0)
                return;

            if (disableOnDied)
                canvasGroup.alpha = 0;
        }

        public void UpdateUI(EnemyObject targetEnemy, int curHp)
        {
            canvasGroup.alpha = 1;

            _curEnemyObject = targetEnemy;
            nameText.text = _curEnemyObject.UnitData.Name;

            hpBarText.text = $"{curHp} / {_curEnemyObject.MaxHp}";
            hpBar.fillAmount = (float)curHp / _curEnemyObject.MaxHp;

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
}