using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class PoolingObject : MonoBehaviour
	{
		private void OnDisable()
		{
			ObjectManager.Instance?.PushObject(gameObject);
		}
	}
}