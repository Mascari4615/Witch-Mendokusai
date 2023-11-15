using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(EquipmentData), menuName = "Variable/EquipmentData")]
public class EquipmentData : ItemData
{
    public Mastery[] Masteries => masteries;
    public Effect[] Effects => effects;
    [SerializeField] private Mastery[] masteries;
    [SerializeField] private Effect[] effects;
}
