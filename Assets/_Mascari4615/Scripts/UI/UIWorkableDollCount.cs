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
				int workableDollCount = SOManager.Instance.Dolls.RuntimeItems.Count;
				foreach (Doll doll in SOManager.Instance.Dolls.RuntimeItems)
				{
					if (doll.ID == Doll.DUMMY_ID)
						continue;

					if (workManager.DoseDollHaveWork(doll.ID))
						workableDollCount--;
				}

				if (workManager.Works.ContainsKey(Doll.DUMMY_ID))
					workableDollCount -= workManager.Works[Doll.DUMMY_ID].Count;

				text.text = $"{workableDollCount}/{SOManager.Instance.Dolls.RuntimeItems.Count} 인형";
				yield return wait;
			}
		}
	}
}