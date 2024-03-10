using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class Trail : MonoBehaviour
	{
		[SerializeField] private TrailRenderer trailRenderer;

		private void OnEnable()
		{
			Clear();
		}

		public void Clear()
		{
			trailRenderer.Clear();
		}
	}
}