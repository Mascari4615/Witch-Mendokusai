using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class ManaUI : MonoBehaviour
	{
		[SerializeField] private IntVariable curMana;
		[SerializeField] private IntVariable maxMana;
		[SerializeField] private TextMeshProUGUI text;
		[SerializeField] private Image manaBar;
		private int curValue;

		private void LateUpdate()
		{
			manaBar.fillAmount = (float)curMana.RuntimeValue / maxMana.RuntimeValue;
			curValue = (int)Mathf.Ceil(Mathf.SmoothStep(curValue, curMana.RuntimeValue, .5f));
			text.text = curValue.ToString("N0") + "마나";
		}
	}
}