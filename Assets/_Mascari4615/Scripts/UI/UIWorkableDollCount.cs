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
				yield return wait;
				
				if (DataManager.Instance.IsInited == false)
					continue;

				int workableDollCount = SOManager.Instance.DollBuffer.Datas.Count;
				workableDollCount -= workManager.GetWorkCount(WorkListType.DollWork) + workManager.GetWorkCount(WorkListType.DummyWork);
				text.text = $"{workableDollCount}/{SOManager.Instance.DollBuffer.Datas.Count} 인형";
			}
		}
	}
}