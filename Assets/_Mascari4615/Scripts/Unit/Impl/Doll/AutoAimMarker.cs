using UnityEngine;

namespace Mascari4615
{
	public class AutoAimMarker : MonoBehaviour
	{
		private Animator animator;
		private Transform lastNearestTarget;

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		private void Update()
		{
			if (Player.Instance.AutoAimPos == Vector3.zero)
			{
				animator.SetBool("ON", false);
				return;
			}

			if (lastNearestTarget != Player.Instance.NearestTarget)
			{
				lastNearestTarget = Player.Instance.NearestTarget;
				animator.SetTrigger("RESET");
			}

			animator.SetBool("ON", true);
			transform.position = Player.Instance.AutoAimPos;
		}
	}
}