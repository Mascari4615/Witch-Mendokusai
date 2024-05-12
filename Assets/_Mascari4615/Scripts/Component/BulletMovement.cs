using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	public class BulletMovement : SkillComponent
	{
		private Vector3 moveDirection;
		[SerializeField] private float moveSpeed;
		[SerializeField] private bool useAutoAim;

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
				if (useAutoAim == false)
				{
					moveDirection = SOManager.Instance.PlayerAimDirection.RuntimeValue;
					moveDirection.y = 0;
				}
				// TODO: Setting, UseAutoAim Option
				else
				{
					moveDirection = (SOManager.Instance.PlayerAutoAimPosition.RuntimeValue - transform.position).normalized;
					moveDirection.y = 0;
				}
			}
			else
			{
				SetMoveDirection((Player.Instance.transform.position - skillObject.User.transform.position).normalized);
			}
		}
	}
}