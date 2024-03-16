using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public abstract class UIPanel : MonoBehaviour
	{
		// private CanvasGroup canvasGroup;
		private bool active;
		
		public virtual void Init() {}

		public abstract void UpdateUI(int[] someData = null);
		public virtual void OnOpen() {}
		public virtual void OnClose() {}

		public void SetActive(bool newActive)
		{
			// Debug.Log($"{name} SetActive {newActive}");
			active = newActive;

			//if (canvasGroup == null && TryGetComponent(out canvasGroup) == false)
			//	canvasGroup = gameObject.AddComponent<CanvasGroup>();

			// canvasGroup.alpha = active ? 1 : 0;
			// canvasGroup.blocksRaycasts = active;
			// canvasGroup.interactable = active;

			gameObject.SetActive(active);

			if (active)
				OnOpen();
			else
				OnClose();
		}

		public void ToggleActive() => SetActive(!active);
	}
}