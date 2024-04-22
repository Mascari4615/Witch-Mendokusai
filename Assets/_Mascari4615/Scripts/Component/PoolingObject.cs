using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Mascari4615.MHelper;

namespace Mascari4615
{
	public class PoolingObject : MonoBehaviour
	{
		private void OnDisable()
		{
			if (IsPlaying)
				ObjectManager.Instance?.PushObject(gameObject);
		}
	}
}