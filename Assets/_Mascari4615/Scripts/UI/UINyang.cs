using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public class UINyang : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI text;
		private int curValue;
		private Coroutine coroutine;

		private void Awake()
		{
			SOManager.Instance.Nyang.GameEvent.AddCallback(UpdateGold);
		}

		private void UpdateGold()
		{
			if (coroutine != null)
				StopCoroutine(coroutine);
			coroutine = StartCoroutine(UpdateGoldCoroutine());
		}

		private IEnumerator UpdateGoldCoroutine()
		{
			int targetValue = SOManager.Instance.Nyang.RuntimeValue;
			while (curValue != targetValue)
			{
				curValue = (int)Mathf.Ceil(Mathf.SmoothStep(curValue, targetValue, .5f));
				text.text = curValue.ToString("N0") + "냥";
				yield return null;
			}
		}
	}
}