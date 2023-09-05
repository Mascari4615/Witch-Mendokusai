using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private BoolVariable canInteract;
    [SerializeField] private BoolVariable isPlayerButton0Down;
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private PlayerObject playerObject;

    public void TeleportTo(Vector3 targetPos)
    {
        transform.position = targetPos;
    }

    public void PlayerButton0()
    {
        if (canInteract.RuntimeValue)
        {
            playerInteraction.Interaction();
        }
        else
        {
            playerObject.UseSkill(0);
        }
    }

    private void Update()
    {
        if (canInteract.RuntimeValue && Input.GetKeyDown(KeyCode.Space))
        {
            playerInteraction.Interaction();
        }
        else if (isPlayerButton0Down.RuntimeValue || Input.GetKey(KeyCode.Space))
        {
            playerObject.UseSkill(0);
        }
    }
}