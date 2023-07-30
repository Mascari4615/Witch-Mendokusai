using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Skill : Artifact
{
    public float Cooltime => cooltime;

    [SerializeField] private float cooltime;
    
    public abstract bool Use(UnitObject unitObject);
}
