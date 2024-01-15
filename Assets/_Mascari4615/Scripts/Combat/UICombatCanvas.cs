using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UICombatCanvas : MonoBehaviour
	{
		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private TextMeshProUGUI timeText;
		[SerializeField] private TextMeshProUGUI levelText;
		[SerializeField] private IntVariable maxExp;
		[SerializeField] private IntVariable curExp;
		[SerializeField] private IntVariable curLevel;
		[SerializeField] private Image expBar;
		[SerializeField] private float expBarLerpSpeed = 1f;

		private DateTime startTime;
		private Coroutine updateExpBarLerpRoutine;

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
			if (updateExpBarLerpRoutine != null)
				StopCoroutine(updateExpBarLerpRoutine);
			updateExpBarLerpRoutine = StartCoroutine(UpdateExpBarLerp());

			levelText.text = curLevel.RuntimeValue.ToString();
		}

		private IEnumerator UpdateExpBarLerp()
		{
			float t = 0;
			float origin = expBar.fillAmount;
			float target = (float)curExp.RuntimeValue / maxExp.RuntimeValue;

			if (origin > target)
				origin = 0;

			while (true)
			{
				expBar.fillAmount = Mathf.Lerp(origin, target, t);
				t += Time.deltaTime * expBarLerpSpeed;
				yield return null;

				if (t >= 1)
				{
					expBar.fillAmount = target;
					break;
				}
			}
		}
	}
}