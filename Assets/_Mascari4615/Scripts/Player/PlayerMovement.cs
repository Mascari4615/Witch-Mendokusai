using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private Rigidbody playerRigidBody;

		private Vector3 lastAim;
		private Vector3 moveDirection;

		[SerializeField] private SpriteRenderer playerSprite;
		[SerializeField] private BoolVariable isPlayerButton0Down;

		private void Start()
		{
			UpdateLookDirection(Vector3.right);
		}

		private void Update()
		{
			if (SOManager.Instance.IsDashing.RuntimeValue)
				return;

			bool d = Input.GetMouseButtonDown(1);

			if (d)
				StartCoroutine(DashLoop());
			else
				TryMove();
		}

		private void UpdateLookDirection(Vector3 newDirection)
		{
			// Debug.Log(newDirection);
			SOManager.Instance.PlayerLookDirection.RuntimeValue = newDirection;
			playerSprite.flipX = SOManager.Instance.PlayerLookDirection.RuntimeValue.x < 0;
		}

		private void FixedUpdate()
		{
			if (!SOManager.Instance.IsChatting.RuntimeValue)
			{
				Vector3 finalVelocity;

				if (SOManager.Instance.IsDashing.RuntimeValue)
					finalVelocity = lastAim * SOManager.Instance.DashSpeed.RuntimeValue;
				else
					finalVelocity = moveDirection * (SOManager.Instance.MovementSpeed.RuntimeValue * ((Input.GetKey(KeyCode.Space) || isPlayerButton0Down.RuntimeValue) ? .5f : 1));

				playerRigidBody.velocity = finalVelocity;
			}
		}

		public void TeleportTo(Vector3 targetPos)
		{
			transform.position = targetPos;
		}

		private void TryMove()
		{
			float h = Input.GetAxisRaw("Horizontal");
			float v = Input.GetAxisRaw("Vertical");

			if (h == 0)
				h = SOManager.Instance.JoystickX.RuntimeValue;
			if (v == 0)
				v = SOManager.Instance.JoystickY.RuntimeValue;

			moveDirection.x = h;
			moveDirection.z = v;

			moveDirection = moveDirection.normalized;

			SOManager.Instance.PlayerMoveDirection.RuntimeValue = moveDirection;

			if (h != 0 || v != 0)
				UpdateLookDirection(moveDirection);
		}

		private IEnumerator DashLoop()
		{
			SOManager.Instance.IsDashing.RuntimeValue = true;
			lastAim = SOManager.Instance.PlayerAimDirection.RuntimeValue;

			float t = 0;
			while (t <= SOManager.Instance.DashDuration.RuntimeValue)
			{
				yield return null;
				t += Time.deltaTime;
			}

			SOManager.Instance.IsDashing.RuntimeValue = false;
		}
	}
}