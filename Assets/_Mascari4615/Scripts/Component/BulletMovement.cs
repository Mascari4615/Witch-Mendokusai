using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public class BulletMovement : SkillComponent
	{
		private Vector3 moveDirection;
		[SerializeField] private float moveSpeed;

		public void SetMoveDirection(Vector3 newDirection)
		{
			moveDirection = newDirection;
		}

		// Update is called once per frame
		private void Update()
		{
			transform.position += moveSpeed * Time.deltaTime * moveDirection;
		}

		public override void InitContext(SkillObject skillObject)
		{
			if (skillObject.UsedByPlayer)
			{
				if (SOManager.Instance.PlayerAimDirection)
				{
					moveDirection = SOManager.Instance.PlayerAimDirection.RuntimeValue;
					moveDirection.y = 0;
				}
				// TODO, Setting, UseAutoAim Option
				else if (SOManager.Instance.PlayerAutoAimDirection && SOManager.Instance.PlayerAutoAimDirection.RuntimeValue != Vector3.zero)
				{
					moveDirection = SOManager.Instance.PlayerAutoAimDirection.RuntimeValue;
					moveDirection.y = 0;
				}
			}
			else
			{
				SetMoveDirection((PlayerController.Instance.transform.position - skillObject.User.transform.position).normalized);
			}
		}
	}
}