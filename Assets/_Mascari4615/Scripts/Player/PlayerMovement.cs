using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace Mascari4615
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private bool useRotate;
		[SerializeField] private SpriteRenderer playerSprite;
		private Rigidbody playerRigidBody;
		private PlayerObject playerObject;

		private Vector3 lastMoveDirection;
		private Vector3 moveDirection;

		private const float ROTATE_SPEED = 150;
		private const float CAMERA_ROTATE_SPEED = 15;
		private float yRotation = 0;

		private void Awake()
		{
			playerRigidBody = GetComponent<Rigidbody>();
			playerObject = GetComponent<PlayerObject>();
		}

		private void Start()
		{
			UpdateLookDirection(Vector3.right);
		}

		private void Update()
		{
			if (SOManager.Instance.IsDashing.RuntimeValue)
				return;

			Rotate();

			// bool d = Input.GetMouseButtonDown(1);
			bool d = Input.GetKeyDown(KeyCode.Space);

			if (d)
				StartCoroutine(DashLoop());
			else
				TryMove();
		}

		private void Rotate()
		{
			if (useRotate == false)
				return;

			if (Input.GetKey(KeyCode.Q))
				yRotation += Time.deltaTime * ROTATE_SPEED;
			if (Input.GetKey(KeyCode.E))
				yRotation -= Time.deltaTime * ROTATE_SPEED;

			Quaternion targetRotation = Quaternion.Euler(0, yRotation, 0);
			// transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5);
			transform.rotation = targetRotation;
			Camera.main.transform.parent.rotation = Quaternion.Lerp(Camera.main.transform.parent.rotation, targetRotation, Time.deltaTime * CAMERA_ROTATE_SPEED);
		}

		private void FixedUpdate()
		{
			if (SOManager.Instance.IsChatting.RuntimeValue)
				return;

			if (SOManager.Instance.IsCooling.RuntimeValue)
			{
				playerRigidBody.velocity = Vector3.zero;
				return;
			}

			Vector3 finalVelocity;

			if (SOManager.Instance.IsDied.RuntimeValue)
				finalVelocity = Vector3.zero;
			else if (SOManager.Instance.IsDashing.RuntimeValue)
				finalVelocity = lastMoveDirection * SOManager.Instance.DashSpeed.RuntimeValue;
			else
				finalVelocity = moveDirection * playerObject.UnitData.MoveSpeed * SOManager.Instance.MovementSpeed.RuntimeValue;

			playerRigidBody.velocity = finalVelocity;
			// playerRigidBody.AddForce(finalVelocity, ForceMode.VelocityChange);
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

			// moveDirection.x = h;
			// moveDirection.z = v;
			moveDirection = (h * transform.right) + (v * transform.forward);
			moveDirection = moveDirection.normalized;

			SOManager.Instance.PlayerMoveDirection.RuntimeValue = moveDirection;

			if (h != 0 || v != 0)
				UpdateLookDirection(moveDirection);
		}

		private void UpdateLookDirection(Vector3 newDirection)
		{
			// Debug.Log(newDirection);
			float h = Input.GetAxisRaw("Horizontal");

			SOManager.Instance.PlayerLookDirection.RuntimeValue = newDirection;
			playerSprite.flipX = h == 0 ? playerSprite.flipX : h < 0;
		}

		private IEnumerator DashLoop()
		{
			SOManager.Instance.IsDashing.RuntimeValue = true;
			lastMoveDirection = SOManager.Instance.PlayerMoveDirection.RuntimeValue;

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