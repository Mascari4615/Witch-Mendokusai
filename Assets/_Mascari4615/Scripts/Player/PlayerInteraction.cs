using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class PlayerInteraction : MonoBehaviour
	{
		[SerializeField] private Collider collider;
		private Dictionary<int, InteractiveObject> nearInterativeObjects = new Dictionary<int, InteractiveObject>();

		private InteractiveObject GetNearestInteractiveObject()
		{
			if (nearInterativeObjects.Count == 0)
				return null;

			var nearest = nearInterativeObjects.Values.OrderBy(
					x => Vector3.Distance(transform.position, x.transform.position))
				.First();
			return nearest;
		}

		private void Start()
		{
			UpdateCanInteractVariable();
		}

		public void Interaction()
		{
			GetNearestInteractiveObject()?.Interact();
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Interactive"))
			{
				if (other.TryGetComponent(out InteractiveObject interactiveObject))
				{
					nearInterativeObjects.Add(other.gameObject.GetInstanceID(), interactiveObject);
					UpdateCanInteractVariable();
				}
			}
		}

		public void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Interactive"))
			{
				if (nearInterativeObjects.ContainsKey(other.gameObject.GetInstanceID()))
				{
					nearInterativeObjects.Remove(other.gameObject.GetInstanceID());
					UpdateCanInteractVariable();
				}
			}
		}

		private void UpdateCanInteractVariable()
		{
			SOManager.Instance.CanInteract.RuntimeValue = nearInterativeObjects.Count > 0;
			// Debug.Log(nameof(UpdateCanInteractVariable) + nearInterativeObjects.Count);
		}

		public void SetLayer(int layer)
		{
			gameObject.layer = layer;
		}
	}
}