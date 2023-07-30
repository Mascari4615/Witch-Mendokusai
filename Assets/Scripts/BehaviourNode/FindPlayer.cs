using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class FindPlayer : ActionNode
{
    [SerializeField] private float sight = 5f;
    
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        return IsPlayerNear() ? State.Success : State.Failure;
    }

    protected bool IsPlayerNear()
    {
        return Vector3.Distance(PlayerController.Instance.transform.position, context.transform.position) < sight;
    }
    
}