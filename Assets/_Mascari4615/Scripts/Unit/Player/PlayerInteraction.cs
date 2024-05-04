using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mascari4615
{
	public class PlayerInteraction
	{
		public const float InteractionDistance = 1.5f;

		private GameObject GetNearestInteractiveObject()
		{
			GameObject nearest = ObjectBufferManager.Instance.GetNearestObject(ObjectType.Interactive, Player.Instance.transform.position, InteractionDistance);
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