using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder
{
    [System.Serializable]
    public class LookPlayer : ActionNode
    {
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            context.spriteRenderer.flipX = IsPlayerOnLeft();
            return State.Success;
        }
        
        protected bool IsPlayerOnLeft()
        {
            return PlayerController.Instance.transform.position.x < context.transform.position.x;
        }
    }
}