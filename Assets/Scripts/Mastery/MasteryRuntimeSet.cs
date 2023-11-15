using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MasteryRuntimeSet", menuName = "GameSystem/RunTimeSet/Mastery")]
public class MasteryRuntimeSet : RuntimeSet<Mastery>
{
    [SerializeField] private bool useEffect;
    
    public override void Add(Mastery mastery)
    {
        base.Add(mastery);
        if (useEffect)
            mastery.OnEquip();
    }

    public override void Remove(Mastery mastery)
    {
        if (useEffect)
            if (Items.Contains(mastery))
                mastery.OnRemove();

        base.Remove(mastery);
    }
}