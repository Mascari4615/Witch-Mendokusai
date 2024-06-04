using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class Rotate : MonoBehaviour
	{
		[SerializeField] private float speed = 100f;

		private void Update()
		{
			transform.Rotate(Vector3.forward, speed * Time.deltaTime);
		}
	}
}