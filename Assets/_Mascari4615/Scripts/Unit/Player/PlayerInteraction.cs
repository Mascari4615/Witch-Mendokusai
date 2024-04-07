using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class PlayerInteraction : MonoBehaviour
	{
		private GameObject GetNearestInteractiveObject()
		{
			const float maxDistance = 1.5f;
			GameObject nearest = GameManager.Instance.GetNearestObject(ObjectBufferType.Interactive, transform.position, maxDistance);
			return nearest;
		}

		private void Start()
		{
			SOManager.Instance.CanInteract.RuntimeValue = true;
		}

		public void Interaction()
		{
			GameObject nearest = GetNearestInteractiveObject();
			if (nearest == null)
				return;
			nearest.GetComponent<InteractiveObject>().Interact();
		}

		private void UpdateCanInteractVariable()
		{
			SOManager.Instance.CanInteract.RuntimeValue = GameManager.Instance.ObjectBuffers[ObjectBufferType.Interactive].Count > 0;
		}

		public void SetLayer(int layer)
		{
			gameObject.layer = layer;
		}
	}
}