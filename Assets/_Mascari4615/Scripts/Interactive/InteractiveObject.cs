using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public abstract class InteractiveObject : MonoBehaviour
	{
		public abstract void Interact();

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