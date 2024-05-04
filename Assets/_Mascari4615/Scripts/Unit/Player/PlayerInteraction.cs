using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class PlayerInteraction
	{
		private GameObject GetNearestInteractiveObject()
		{
			const float maxDistance = 1.5f;
			GameObject nearest = ObjectBufferManager.Instance.GetNearestObject(ObjectType.Interactive, Player.Instance.transform.position, maxDistance);
			return nearest;
		}

		public void TryInteraction()
		{
			GameObject nearest = GetNearestInteractiveObject();
			if (nearest == null)
				return;
			nearest.GetComponent<InteractiveObject>().Interact();
		}
	}
}