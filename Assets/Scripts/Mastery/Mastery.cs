using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(Mastery), menuName = "Variable/Mastery")]
public class Mastery : Artifact
{
    public Effect[] Effects => effects;
    [SerializeField] private Effect[] effects;

    public void OnEquip()
    {
        foreach (var effect in effects)
            effect.OnEquip();
    }
    
    public void OnRemove()
    {
        foreach (var effect in effects)
            effect.OnRemove();
    }
}
