using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
    public class UIBar : MonoBehaviour
    {
		[SerializeField] private TextMeshProUGUI text;
		[SerializeField] private IntVariable textTarget;

		[SerializeField] private Image bar;
		[SerializeField] private IntVariable cur;
		[SerializeField] private IntVariable max;
		[SerializeField] private float lerpSpeed = 5f;

		[SerializeField] private bool isExpBar;

		private Coroutine routine;

		private void OnEnable()
		{
			UpdateUI();
		}

		public void UpdateUI()
		{
			if (routine != null)
				StopCoroutine(routine);
			routine = StartCoroutine(UpdateBarLerp());

			text.text = textTarget.RuntimeValue.ToString();
		}

		private IEnumerator UpdateBarLerp()
		{
			float t = 0;
			float origin = bar.fillAmount;
			float target = (float)cur.RuntimeValue / max.RuntimeValue;

			if (isExpBar)
				if (origin > target)
					origin = 0;

			while (true)
			{
				bar.fillAmount = Mathf.Lerp(origin, target, t);
				t += Time.deltaTime * lerpSpeed;
				yield return null;

				if (t >= 1)
				{
					bar.fillAmount = target;
					break;
				}
			}
		}
	}
}