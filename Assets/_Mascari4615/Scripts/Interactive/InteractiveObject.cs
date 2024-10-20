using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class InteractiveObject : MonoBehaviour
	{
		public static readonly List<InteractiveObject> ActiveInteractives = new();

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
			ActiveInteractives.Add(this);
		}

		private void OnDisable()
		{
			if (MHelper.IsPlaying)
				ActiveInteractives.Remove(this);
		}
	}
}