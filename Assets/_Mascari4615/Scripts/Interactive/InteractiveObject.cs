using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mascari4615
{
	public interface IInteractable
	{
		void OnInteract();
	}

	public class InteractiveObject : MonoBehaviour
	{
		private IInteractable[] interactable;

		public void Interact()
		{
			foreach (IInteractable interact in interactable)
				interact.OnInteract();
		}

		private void Awake()
		{
			interactable = GetComponents<IInteractable>();
		}

		private void OnEnable()
		{
			SOManager.Instance.InteractiveObjectBuffer.AddItem(gameObject);
		}

		private void OnDisable()
		{
			SOManager.Instance.InteractiveObjectBuffer.RemoveItem(gameObject);
		}
	}
}