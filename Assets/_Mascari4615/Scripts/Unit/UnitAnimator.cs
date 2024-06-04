using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class UnitAnimator : MonoBehaviour
	{
		[SerializeField] private Animator mainAnimator;
		[SerializeField] private Transform pivot;

		[SerializeField] private Rigidbody rigidbody;

		[SerializeField] private Animator handAnimator;

		private void Update()
		{
			if (rigidbody.velocity.magnitude > 0.1f)
				mainAnimator.SetBool("MOVE", true);
			else
				mainAnimator.SetBool("MOVE", false);

			if (rigidbody.velocity.x > 0)
				pivot.localScale = new Vector3(1, 1, 1);
			else if (rigidbody.velocity.x < 0)
				pivot.localScale = new Vector3(-1, 1, 1);

			handAnimator.SetBool("CHANNELING", Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1));
		}
	}
}