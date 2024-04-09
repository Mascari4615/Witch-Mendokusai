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

		private UnitObject targetUnit;

		private void OnEnable()
		{
			UpdateUI();
		}

		public void UpdateUI()
		{
			canvasGroup.alpha = 1;

			targetUnit = lastHitEnemyObject.RuntimeValue;
			nameText.text = targetUnit.UnitData.Name;

			hpBarText.text = $"{targetUnit.Stat[StatType.HP_CUR]} / {targetUnit.Stat[StatType.HP_MAX]}";
			hpBar.fillAmount = (float)targetUnit.Stat[StatType.HP_CUR] / targetUnit.Stat[StatType.HP_MAX];

			if (targetUnit.Stat[StatType.HP_CUR] != 0)
				return;

			if (disableOnDied)
				canvasGroup.alpha = 0;
		}

		public void UpdateUI(MonsterObject targetEnemy, int curHp)
		{
			canvasGroup.alpha = 1;

			targetUnit = targetEnemy;
			nameText.text = targetUnit.UnitData.Name;

			hpBarText.text = $"{curHp} / {targetUnit.Stat[StatType.HP_MAX]}";
			hpBar.fillAmount = (float)curHp / targetUnit.Stat[StatType.HP_MAX];

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