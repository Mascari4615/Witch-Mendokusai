using System;
using System.Collections;
using System.Collections.Generic;
using Karmotrine.Script;
using UnityEngine;

public interface IHitable
{
    public void ReceiveAttack(int damage);
}

public class DamagingObject : SkillComponent
{
    [SerializeField] private int damage;

    private bool usedByPlayer = false;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IHitable hitable))
        {
            if (hitable is EnemyObject && usedByPlayer)
            {
                Debug.Log(nameof(OnTriggerEnter));
                hitable.ReceiveAttack(damage);
            }
            else if (hitable is PlayerObject && !usedByPlayer)
            {
                Debug.Log(nameof(OnTriggerEnter));
                hitable.ReceiveAttack(damage);
            }
        }
    }

    public override void InitContext(SkillObject skillObject)
    {
        usedByPlayer = skillObject.UsedByPlayer;
    }
}
