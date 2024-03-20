using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mascari4615
{
	public class UIWorkableDollCount : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI text;
		private Coroutine coroutine;
		private WorkManager workManager;

		private void OnEnable()
		{
			workManager = DataManager.Instance.WorkManager;
			coroutine = StartCoroutine(UpdateUI());
		}

		private void OnDisable()
		{
			StopCoroutine(coroutine);
		}

		private IEnumerator UpdateUI()
		{
			WaitForSeconds wait = new(.1f);
			while (true)
			{
				int workableDollCount = SOManager.Instance.DollBuffer.RuntimeItems.Count;
				foreach (Doll doll in SOManager.Instance.DollBuffer.RuntimeItems)
				{
					if (doll.ID == Doll.DUMMY_ID)
						continue;

					if (workManager.TryGetWorkByDollID(doll.ID, out _))
						workableDollCount--;
				}

				workableDollCount -= workManager.DummyWorks.Count;

				text.text = $"{workableDollCount}/{SOManager.Instance.DollBuffer.RuntimeItems.Count} 인형";
				yield return wait;
			}
		}
	}
}