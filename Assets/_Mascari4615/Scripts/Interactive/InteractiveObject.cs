using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
			GameManager.Instance.InteractiveObjects.Add(gameObject);
		}

		private void OnDisable()
		{
			if (SceneManager.GetActiveScene().isLoaded == false || Application.isPlaying == false)
			{
				// Debug.LogWarning("Scene is not loaded");
				return;
			}

			GameManager.Instance.InteractiveObjects.Remove(gameObject);
		}
	}
}