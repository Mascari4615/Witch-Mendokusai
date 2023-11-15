using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(StatDicEffect), menuName = "Effect/StatDicEffect")]
public class StatDicEffect : Effect
{
    [SerializeField] private StatDictionary statDictionary;
    [SerializeField] private string stat;
    [SerializeField] private int value;
    
    public override void OnEquip()
    {
        statDictionary.SetStat(stat, value);
    }

    public override void OnRemove()
    {
        statDictionary.SetStat(stat, value);
    }
}
