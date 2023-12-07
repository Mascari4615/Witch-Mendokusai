using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class StageObject : MonoBehaviour
	{
		public Portal[] Portals => portals;
		[SerializeField] private Portal[] portals;

		private void OnEnable()
		{
			foreach (var portal in portals)
				portal.Active();
		}
	}
}