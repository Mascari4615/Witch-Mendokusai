using System;
using System.Collections;
using System.Collections.Generic;
using Karmotrine.Script;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoneObject : EnemyObject
{
    private int _life;

    public override void Init(Enemy enemy)
    {
        _life = ((StoneData)enemy).life;
        base.Init(enemy);
    }

    protected override void Die()
    {
        _life--;

        if (_life == 0)
            base.Die();
        else
            SetHp(curEnemy.maxHp);
    }
}