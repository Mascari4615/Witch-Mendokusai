using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public class BulletMovement : SkillComponent
	{
		[SerializeField] private Vector3 moveDirection;
		[SerializeField] private float moveSpeed;

		[SerializeField] private Vector3Variable playerAimDirection;
		[SerializeField] private Vector3Variable playerAutoAimDirection;

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
				if (playerAimDirection)
				{
					moveDirection = playerAimDirection.RuntimeValue;
					moveDirection.y = 0;
				}
				else if(playerAutoAimDirection && playerAutoAimDirection.RuntimeValue != Vector3.zero)
				{
					moveDirection = playerAutoAimDirection.RuntimeValue;
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