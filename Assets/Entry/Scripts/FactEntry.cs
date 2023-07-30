using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(FactEntry), menuName = "Variable/Entry/FactEntry")]
public class FactEntry : BaseEntry
{
    public int Value => value;
    
    [SerializeField] private int value;
    // TODO : Scope
}
