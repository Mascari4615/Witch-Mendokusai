using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class UseSkill : ActionNode
{
    [SerializeField] private int skillButtonIndex;
    
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        context.enemyObject.UseSkill(skillButtonIndex);
        return State.Success;
    }
}