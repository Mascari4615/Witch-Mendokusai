using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private Rigidbody playerRigidBody;

    [SerializeField] private FloatVariable joystickX;
    [SerializeField] private FloatVariable joystickY;

    [SerializeField] private Vector3Variable playerMoveDirection;
    [SerializeField] private Vector3Variable playerLookDirection;

    private Vector3 moveDirection;

    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private BoolVariable isPlayerButton0Down;

    [SerializeField] private BoolVariable isChatting;

    private void Start()
    {
        UpdateLookDirection(Vector3.right);
    }

    private void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        if (h == 0)
            h = joystickX.RuntimeValue;
        if (v == 0)
            v = joystickY.RuntimeValue;

        moveDirection.x = h;
        moveDirection.z = v;

        moveDirection = moveDirection.normalized;

        playerMoveDirection.RuntimeValue = moveDirection;

        if (h != 0 || v != 0)
            UpdateLookDirection(moveDirection);
    }

    private void UpdateLookDirection(Vector3 newDirection)
    {
        playerLookDirection.RuntimeValue = newDirection;
        playerSprite.flipX = playerLookDirection.RuntimeValue.x < 0;
    }

    private void FixedUpdate()
    {
        if (!isChatting.RuntimeValue)
            playerRigidBody.velocity = moveDirection * (movementSpeed * ((Input.GetKey(KeyCode.Space) || isPlayerButton0Down.RuntimeValue) ? .5f : 1));
    }

    public void TeleportTo(Vector3 targetPos)
    {
        transform.position = targetPos;
    }
}