using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : InteractiveObject
{
    public Transform TpPos => tpPos;

    [SerializeField] private Stage targetStage;
    [SerializeField] private int targetPortalIndex;
    [SerializeField] private Transform tpPos;

    public override void Interact()
    {
        StageManager.Instance.LoadStage(targetStage, targetPortalIndex);
    }

    public void Active()
    {
        gameObject.layer = 0;
    }
}