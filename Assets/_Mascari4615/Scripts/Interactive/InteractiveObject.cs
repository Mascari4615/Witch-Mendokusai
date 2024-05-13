using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static Mascari4615.MHelper;

namespace Mascari4615
{
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
			ObjectBufferManager.Instance.AddObject(ObjectType.Interactive, gameObject);
		}

		private void OnDisable()
		{
			if (IsPlaying)
				ObjectBufferManager.Instance.RemoveObject(ObjectType.Interactive, gameObject);
		}
	}
}