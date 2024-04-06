using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIPopup : MonoBehaviour
	{
		[SerializeField] private UISlot slot;
		[SerializeField] private Animator animator;

		private readonly Queue<DataSO> dataSOs = new();
		private Coroutine coroutine;

		public void Popup(DataSO dataSO)
		{
			dataSOs.Enqueue(dataSO);

			if (coroutine == null)
				coroutine = StartCoroutine(PopupCoroutine());
		}

		private IEnumerator PopupCoroutine()
		{
			while (dataSOs.Count > 0)
			{
				DataSO targetDataSO = dataSOs.Dequeue();

				slot.SetSlot(targetDataSO);
				animator.SetTrigger("POP");
				yield return new WaitForSecondsRealtime(animator.GetCurrentAnimatorStateInfo(0).length);
			}
		}
	}
}