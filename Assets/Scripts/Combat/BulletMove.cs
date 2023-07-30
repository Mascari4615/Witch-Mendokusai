using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BulletMove : SkillComponent
{
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private float moveSpeed;

    [SerializeField] private Vector3Variable playerLookDirection;
    [SerializeField] private Vector3Variable playerAutoAimDirection;

    public void SetMoveDirection(Vector3 newDirection)
    {
        moveDirection = newDirection;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    public override void InitContext(SkillObject skillObject)
    {
        if (skillObject.UsedByPlayer)
        {
            if (playerAutoAimDirection)
            {
                if (playerAutoAimDirection.RuntimeValue != Vector3.zero)
                {
                    moveDirection = playerAutoAimDirection.RuntimeValue;
                    return;
                }
            }

            if (playerLookDirection)
            {
                moveDirection = playerLookDirection.RuntimeValue;
            }
        }
        else
        {
            SetMoveDirection((PlayerController.Instance.transform.position - skillObject.User.transform.position).normalized);
        }
    }
}