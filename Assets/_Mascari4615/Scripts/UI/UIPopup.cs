using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UIPopup : MonoBehaviour
	{
		[SerializeField] private UISlot slot;
		[SerializeField] private Animator animator;

		private readonly Queue<Artifact> artifacts = new();
		private Coroutine coroutine;

		public void Popup(Artifact artifact)
		{
			artifacts.Enqueue(artifact);

			if (coroutine == null)
				coroutine = StartCoroutine(PopupCoroutine());
		}

		private IEnumerator PopupCoroutine()
		{
			while (artifacts.Count > 0)
			{
				Artifact targetArtifact = artifacts.Dequeue();

				slot.SetArtifact(targetArtifact);
				animator.SetTrigger("POP");
				yield return new WaitForSecondsRealtime(animator.GetCurrentAnimatorStateInfo(0).length);
			}
		}
	}
}