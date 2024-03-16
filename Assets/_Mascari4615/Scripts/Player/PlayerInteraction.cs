using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class PlayerInteraction : MonoBehaviour
	{
		private InteractiveObject GetNearestInteractiveObject()
		{
			if (SOManager.Instance.InteractiveObjectBuffer.RuntimeItems.Count == 0)
				return null;

			GameObject nearest = SOManager.Instance.InteractiveObjectBuffer.RuntimeItems.OrderBy(
					x => Vector3.Distance(transform.position, x.transform.position))
				.First();
			return nearest.GetComponent<InteractiveObject>();
		}

		private void Start()
		{
			SOManager.Instance.CanInteract.RuntimeValue = true;
		}

		public void Interaction()
		{
			GetNearestInteractiveObject()?.Interact();
		}

		private void UpdateCanInteractVariable()
		{
			SOManager.Instance.CanInteract.RuntimeValue = SOManager.Instance.InteractiveObjectBuffer.RuntimeItems.Count > 0;
		}

		public void SetLayer(int layer)
		{
			gameObject.layer = layer;
		}
	}
}